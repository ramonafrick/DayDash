# C# & .NET 10 Standards
- **Null Safety:** Use nullable reference types. Use `is not null` instead of `!= null`.
- **Modern C#:** Prefer `var` when type is obvious. Use `String.Equals` with `StringComparison.OrdinalIgnoreCase`.
- **Performance:** Use `ValueTask` for methods that often return synchronously. 
- **Error Handling:** Don't catch generic `Exception`; catch specific types. Use `throw;` to preserve stack trace.
