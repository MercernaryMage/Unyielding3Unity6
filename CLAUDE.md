Before writing new logic, always keep in mind the shared helpers in `Assets/Utility/Util.cs`, `Assets/Grid/TileGrid.cs`, `Assets/Grid/TemplateLibrary.cs`, and `Assets/AI/Card.cs`. They contain the project's reusable utilities (dice/shuffle/routing helpers, grid and adjacency queries, targeting templates, and card lifecycle/flow helpers) — prefer reusing them over hand-rolling equivalent code.


Do not use properties ever for any reason.