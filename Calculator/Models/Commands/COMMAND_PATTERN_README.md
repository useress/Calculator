# Command Pattern Implementation in Calculator

## Overview
The calculator application has been refactored to use the **Command Pattern**, a behavioral design pattern that encapsulates requests as objects. This allows us to:

- ✅ Parameterize clients with different requests
- ✅ Queue requests for execution
- ✅ Support undo/redo operations
- ✅ Log and audit operations
- ✅ Support deferred execution
- ✅ Create macros/scripts of operations

## Architecture

### Key Components

#### 1. **ICalculatorCommand** (Core Interface)
```csharp
public interface ICalculatorCommand
{
    void Execute();
    void Undo();
    bool CanUndo { get; }
}
```
- Defines the contract for all calculator commands
- Enforces implementation of Execute and Undo methods
- Allows polymorphic treatment of different operations

#### 2. **Concrete Commands**
Each calculator operation is encapsulated in its own command class:

- **InputCommand** - Handles user input (digits, operators, functions)
- **ClearCommand** - Clears the display
- **BackspaceCommand** - Removes the last character
- **CalculateCommand** - Evaluates the expression
- **ToggleLayoutCommand** - Switches between normal and extended layout

Each command stores the necessary state to support undo operations:
```csharp
public class InputCommand : ICalculatorCommand
{
    private string _previousDisplay;  // Store state for undo
    
    public void Execute() { /* new state */ }
    public void Undo() { /* restore previous state */ }
}
```

#### 3. **CalculatorState**
- Represents the mutable state of the calculator
- Implements INotifyPropertyChanged for MVVM binding
- Properties: Display, IsExtendedLayout
- Shared across all commands

#### 4. **CommandInvoker**
- Manages command execution
- Maintains undo/redo stacks
- Provides unified interface for operation execution
- Fires events when undo/redo availability changes

```csharp
public class CommandInvoker
{
    private Stack<ICalculatorCommand> _undoStack;
    private Stack<ICalculatorCommand> _redoStack;
    
    public void ExecuteCommand(ICalculatorCommand command);
    public void Undo();
    public void Redo();
    public bool CanUndo { get; }
    public bool CanRedo { get; }
}
```

#### 5. **CalculatorViewModel** (Refactored)
- Creates command instances
- Delegates to CommandInvoker for execution
- Exposes UndoCommand and RedoCommand to UI
- Maintains CanUndo/CanRedo properties for UI binding

## Data Flow

### Execution Flow
```
User Action (Click Button)
    ↓
ViewModel Command (InputCommand, ClearCommand, etc.)
    ↓
Create Concrete Command with State
    ↓
CommandInvoker.ExecuteCommand()
    ↓
Command.Execute() - Modifies CalculatorState
    ↓
State Properties Raise PropertyChanged Events
    ↓
UI Updates via MVVM Binding
```

### Undo Flow
```
User Clicks Undo Button
    ↓
ViewModel.UndoCommand Executed
    ↓
CommandInvoker.Undo()
    ↓
Pop Command from Undo Stack
    ↓
Command.Undo() - Restores Previous State
    ↓
Command Pushed to Redo Stack
    ↓
State Updates via PropertyChanged
    ↓
UI Reflects Previous State
```

## Benefits

### 1. **Separation of Concerns**
- Commands encapsulate what to do
- CommandInvoker manages how to do it
- CalculatorState manages the data
- ViewModel orchestrates the flow

### 2. **Undo/Redo Support**
- Each command knows how to undo itself
- CommandInvoker maintains history
- No need for complex state snapshots

### 3. **Extensibility**
- Adding new commands is simple
- Just implement ICalculatorCommand
- No changes needed to core infrastructure

Example:
```csharp
public class MyNewCommand : ICalculatorCommand
{
    private CalculatorState _state;
    private string _previousState;
    
    public void Execute() { /* implementation */ }
    public void Undo() { /* restoration */ }
    public bool CanUndo => true;
}

// Use it:
var command = new MyNewCommand(_state);
_commandInvoker.ExecuteCommand(command);
```

### 4. **Testability**
- Commands can be tested in isolation
- No need for UI for testing
- Easy to create command sequences for testing

### 5. **Logging and Auditing**
- Can easily log all executed commands
- Can create audit trails
- Can replay sequences of operations

## Usage Example

### In ViewModel
```csharp
private void ExecuteInputCommand(string input)
{
    if (string.IsNullOrEmpty(input))
        return;

    // Create command
    var command = new InputCommand(_state, input);
    
    // Execute through invoker
    _commandInvoker.ExecuteCommand(command);
    
    // CommandInvoker automatically:
    // - Executes the command
    // - Adds to undo stack
    // - Clears redo stack
    // - Fires UndoRedoChanged event
}
```

### Undo/Redo in UI
```csharp
// Bind to ViewModel commands
public ICommand UndoCommand { get; }
public ICommand RedoCommand { get; }

// Bind to ViewModel properties
public bool CanUndo { get; set; }
public bool CanRedo { get; set; }

// In XAML
<Button Command="{Binding UndoCommand}" 
        IsEnabled="{Binding CanUndo}" 
        Content="Undo" />
<Button Command="{Binding RedoCommand}" 
        IsEnabled="{Binding CanRedo}" 
        Content="Redo" />
```

## File Structure
```
Calculator/
├── Models/
│   ├── Commands/                  # Command Pattern Implementation
│   │   ├── ICalculatorCommand.cs  # Command Interface
│   │   ├── CalculatorState.cs     # Shared State
│   │   ├── CommandInvoker.cs      # Invoker/Manager
│   │   ├── InputCommand.cs        # Concrete Command
│   │   ├── ClearCommand.cs        # Concrete Command
│   │   ├── BackspaceCommand.cs    # Concrete Command
│   │   ├── CalculateCommand.cs    # Concrete Command
│   │   └── ToggleLayoutCommand.cs # Concrete Command
│   ├── CalculatorModel.cs
│   ├── EngineeringCalculatorModel.cs
│   └── Buttons/                   # Button Pattern
├── ViewModels/
│   ├── CalculatorViewModel.cs     # Refactored to use Commands
│   └── RelayCommand.cs            # Legacy (can be kept or removed)
└── Views/
    └── MainWindow.xaml
```

## Advantages Over Direct Manipulation

### Before (Direct State Modification)
```csharp
// Display directly modified
Display = Display + input;

// Hard to track
// No undo capability
// Business logic scattered
```

### After (Command Pattern)
```csharp
// Encapsulated operation
var command = new InputCommand(_state, input);
_commandInvoker.ExecuteCommand(command);

// Easy to track, undo, extend
// Business logic centralized
// Testable in isolation
```

## Future Enhancements

1. **Command History UI**
   - Display executed commands
   - Allow reverting to any point in history
   - Show command descriptions

2. **Macro Recording**
   - Record sequence of commands
   - Replay recorded sequences
   - Save/load macros

3. **Command Queuing**
   - Queue commands for batch execution
   - Async command execution
   - Progress tracking

4. **Persistence**
   - Save calculation history
   - Load previous sessions
   - Export operation logs

5. **Composite Commands**
   - Group multiple commands
   - Execute as single transaction
   - Undo/redo as unit

## Summary
The Command Pattern provides a clean, extensible architecture for the calculator that makes it easy to add features like undo/redo, logging, and macro recording without cluttering the core logic.
