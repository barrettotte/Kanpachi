
SshClient.cs
```cs

  // creates new SshClient instance
  public SshClient(ConnectionInfo connectionInfo);     

  // creates the command to be executed
  public SshCommand CreateCommand(string commandText);  
  public SshCommand CreateCommand(string commandText, Encoding encoding);

  // creates shell
  public Shell CreateShell(Stream input, Stream output, Stream extendedOutput);
  public Shell CreateShell(
    Encoding encoding, string input, Stream output, Stream extendedOutput, 
    string terminalName, uint columns, uint rows, uint width, uint height, 
    IDictionary<TerminalModes, uint> terminalModes, int bufferSize);
  public Shell CreateShell(Encoding encoding, string input, Stream output, Stream extendedOutput);

  // create shell stream - ```infocmp```
  public ShellStream CreateShellStream(
    string terminalName, uint columns, uint rows, uint width, uint height, 
    int bufferSize, IDictionary<TerminalModes, uint> terminalModeValues);

  // creates and runs command (async internal)
  public SshCommand RunCommand(string commandText);

```


SshCommand.cs
```cs
public string Error;                 // get
public Stream ExtendedOutputStream;  // get
public Stream OutputStream;          // get
public int ExitStatus;               // get
public TimeSpan CommandTimeout;      // get/set
public string CommandText;           // get
public string Result;                // get

// async command execution
// Exceptions:
//   T:System.InvalidOperationException:
//     Asynchronous operation is already in progress.
//   T:System.InvalidOperationException:
//     Asynchronous operation is already in progress.
//   T:Renci.SshNet.Common.SshException:
//     Invalid operation.
//   T:System.ArgumentException:
//     CommandText property is empty.
//   T:System.ArgumentException:
//     CommandText property is empty.
//   T:Renci.SshNet.Common.SshConnectionException:
//     Client is not connected.
//   T:Renci.SshNet.Common.SshOperationTimeoutException:
//     Operation has timed out.
public IAsyncResult BeginExecute();
public IAsyncResult BeginExecute(string commandText, AsyncCallback callback, object state);
public IAsyncResult BeginExecute(AsyncCallback callback, object state);
public IAsyncResult BeginExecute(AsyncCallback callback);

// cancel command exec
public void CancelAsync();

// finish command exec
// Exceptions:
//   T:System.ArgumentException:
//     Either the IAsyncResult object did not come from the corresponding async method
//     on this type, or EndExecute was called multiple times with the same IAsyncResult.
//   T:System.ArgumentNullException:
//     asyncResult is null.
public string EndExecute(IAsyncResult asyncResult);


// execute command
public string Execute();
public string Execute(string commandText);
```