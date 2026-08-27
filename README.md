Event Management Platform

Event management platform built with ASP.NET Core MVC — a portfolio project with a layered architecture (Model → Repository → Service → Web), Identity-based authentication, and an automatic waitlist promotion system.

Features
Event lifecycle: Draft → Published → Cancelled, with role-based permissions
Registration with capacity limits; automatic waitlist + promotion when a spot opens
Admin panel, "My Events", and "My Registrations" views, all paginated
10 event categories

Tech Stack
ASP.NET Core MVC · Entity Framework Core · ASP.NET Core Identity · SQL Server · Bootstrap

Architecture
EventManagement.Data      → Entities, Repositories, DbContext
EventManagement.Services  → Business logic, DTOs
EventManagement.Web       → Controllers, ViewModels, Views
