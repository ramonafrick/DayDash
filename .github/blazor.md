# Blazor Hybrid Best Practices
- **Rendering:** Avoid unnecessary re-renders; use `ShouldRender` if performance is critical.
- **Async:** Always use `Task.Run` for CPU-intensive work to keep the UI responsive.
- **State:** Use a `StateContainer` pattern for cross-component state, don't rely solely on `CascadingParameters`.
- **Interop:** Keep JS-Interop to a minimum; use C#/MAUI APIs wherever possible.
