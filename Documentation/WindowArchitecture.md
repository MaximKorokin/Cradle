# Window Architecture

## Flow

Caller publishes `WindowOpenRequest` or `WindowToggleRequest` -> `WindowsUISystem` -> `WindowManager` -> resolve definition, controller, and prefab -> create/bind window and wrapper. Close requests return through the event bus; the manager hides and disposes the window.

## Types and dependencies

| Type | Depends on |
|---|---|
| `WindowId`, `WindowConfiguration` | None; identify a window and its behavior. |
| `WindowDefinition` | `WindowId`, controller type, `WindowConfiguration`. |
| `WindowPrefabsConfig` | `UIWindowBase` prefab assets. |
| Concrete `UIWindowBase` | Child view components, if any. |
| `WindowControllerBase<TWindow,TArguments>` | Concrete window and `IWindowControllerArguments`. |
| `SingleViewWindowControllerBase` / `MultiViewWindowControllerBase` | Window controller, view controller(s), data aggregator(s), and window view(s). |
| Concrete window controller | Its window, argument type, view controller(s), and data aggregator(s); resolved by VContainer. |
| Argument struct | Caller-provided data; implements `IWindowControllerArguments` (or use `EmptyWindowControllerArguments`). |
| `UIViewBase<TData>` | Data aggregator and Unity child components. |
| `ViewControllerBase<TView,TData>` | View, data aggregator, and presentation services. |
| Data aggregator | Domain data/services used to provide view data. |
| `WindowManager` | `UIRootReferences`, definitions, prefab config, wrappers, VContainer `IObjectResolver`. |
| `WindowsUISystem` | `IGlobalEventBus`, `WindowManager`, window/pointer events. |
| `WindowWrapperBase` | `IGlobalEventBus`, `UIWindowBase`, serialized `RectTransform`. |
| `UILifetimeScope` | UI scene references/prefabs and VContainer; registers systems, definitions, controllers, and data. |

## Add a window

- [ ] Add a unique `WindowId` and definition in `WindowDefinition.cs` / `UILifetimeScope.RegisterWindows()`.
- [ ] Create the `UIWindowBase` component and prefab; wire its child views.
- [ ] Add the prefab to `WindowPrefabsConfig`; its component type must match controller `WindowType`.
- [ ] Create the controller and argument type; register the controller in `UILifetimeScope.RegisterWindows()`.
- [ ] For data-backed views, create view, view controller, and data aggregator; register them in `RegisterViewControllers()` and `RegisterDataAggregators()`.
- [ ] Choose singleton/modal/movable flags; ensure scene roots and wrapper prefabs are configured.
- [ ] Publish an open/toggle request with the new ID and required arguments.
