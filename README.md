# User Access Request (UAR)

This repository contains a sample ASP.NET Core Razor Pages application that replaces a SharePoint-based User Access Request form. It captures request details, approvals, and logs requests in a SQL database.

## Features
- Long-form layout with collapsible sections for general request info, approvals, and access areas based on the provided field list.
- Required fields are marked with an asterisk.
- Automatic tracking of systems that require second-level RDO approval (systems labeled with `RDO`).
- Admin pages to manage static dropdown values (companies, employee status/type, device type, status values, Office365 licenses, BI roles, Pharmerica roles), programs, and users.
- Separate lookup tables and services for users and programs.

## Project Structure
- `UAR.Web/` - ASP.NET Core Razor Pages application.
- `sql/` - SQL scripts to create tables and stored procedures.

## Getting Started
1. Create the database and stored procedures:
   - Run `sql/001_create_tables.sql`.
   - Run `sql/002_stored_procedures.sql`.
2. Update the connection string in `UAR.Web/appsettings.json`.
3. Seed dropdown options (`DropdownOptions` table) and programs (`Programs` table).

## Notes
- The form labels/descriptions are placeholders based on common User Access Request fields. Replace them with exact labels from your existing form once available.
- Systems that require second-level RDO approval should include the text `RDO` in the dropdown value so the request summary highlights them.
