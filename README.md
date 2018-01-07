# ETW2Grid
ETW2Grid is a tool that shows recent ETW events from a log file (.ETL) in a grid.
The log file can be a closed file or a file from a current live log.
The implementation uses the ETW deserializer from ETW2JSON.
The event grid has a context menu with group, filter and refresh.

## Command-line usage
``ETW2Grid LogFileName [number of buffers]``

## Details
The number of buffers are the recent ETW buffers written to the file, the default is 10.
This limmits the number of shown events because the log file can be very large.
The number of events in a buffer depends on the buffer size, event size und flush rates.
