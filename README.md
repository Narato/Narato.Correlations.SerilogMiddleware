# Narato.Correlations.SerilogMiddleware
This library contains Middleware for Serilog for automatically enriching log messages with a correlation ID

Getting started
==========
### 1. Add dependency in project.json

```json
"dependencies": {
   "Narato.Correlations": "1.0.0",
   "Narato.Correlations.SerilogMiddleware": "1.0.0" 
}
```

### 2. Configure Serilog.
For basic information on how to setup Serilog, [go here](https://github.com/serilog/serilog).

Please note the `{CorrelationId}` part. This will actually be replaced with the correlation ID.
```C#
var log = new LoggerConfiguration()
    .WriteTo.RollingFile("log-{Date}.txt", "[{Timestamp:HH:mm:ss} {Level} {CorrelationId}] {Message}{NewLine}{Exception}")
    .CreateLogger();
```

### 3. Configure Startup.cs
In the Configure section, add following line
`app.UseCorrelationLogging();`
**BELOW** the configuration of [Narato.Correlations](https://github.com/Narato/Narato.Correlations) (this is required)

# Helping out

If you want to help out, please read [this wiki page](https://github.com/Narato/Narato.Correlations.SerilogMiddleware/wiki/Helping-out)