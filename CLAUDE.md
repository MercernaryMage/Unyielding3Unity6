Before writing new logic, always keep in mind the shared helpers in `Assets/Utility/Util.cs`, `Assets/Grid/TileGrid.cs`, and `Assets/Grid/TemplateLibrary.cs`. They contain the project's reusable utilities (dice/shuffle/routing helpers, grid and adjacency queries, and targeting templates) — prefer reusing them over hand-rolling equivalent code.


Do not use the lambda arrow function ever.