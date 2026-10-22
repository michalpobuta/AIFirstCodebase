---
applyTo: "src/Shop/Features/Warehouse/**"
---
# Warehouse export — module note

## Quick commands
- `dotnet test --filter-class "*WarehouseExportTests"`

## Non-obvious
- The warehouse system polls `/warehouse/export.csv` and parses it with its own parser. Column order,
  separator and status codes are a contract.            [code: WarehouseExport.cs L7–L10, 2026-10]
- Existing status codes never change. A new status needs a code agreed with the warehouse owners and a
  human-approved snapshot.                              [review: CODEOWNERS, 2026-10]
- `CodeFor` has no `_ =>` arm on purpose: a new OrderStatus must fail the build here.   [enforced: CS8509]

## See also
- The approved format: `WarehouseExportTests.ToCsv_OrdersInEveryStatus_MatchesTheFormatTheWarehouseParses.verified.txt`
