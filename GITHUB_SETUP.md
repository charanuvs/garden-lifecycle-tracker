# GitHub Repository Setup Instructions

Since GitHub CLI (`gh`) is not installed on your system, follow these steps to create a GitHub repository manually:

## Option 1: Using GitHub Website

1. **Go to GitHub and create a new repository:**
   - Visit https://github.com/new
   - Repository name: `garden-lifecycle-tracker` (or your preferred name)
   - Description: "ASP.NET Core MVC application for managing garden crop lifecycles using finite state machines"
   - Choose Public or Private
   - **Do NOT** initialize with README, .gitignore, or license (we already have these)
   - Click "Create repository"

2. **Add the remote and push your code:**
   ```bash
   git remote add origin https://github.com/YOUR_USERNAME/garden-lifecycle-tracker.git
   git branch -M main
   git push -u origin main
   ```

## Option 2: Install GitHub CLI (Recommended for Future Use)

1. **Install GitHub CLI on macOS:**
   ```bash
   brew install gh
   ```

2. **Authenticate with GitHub:**
   ```bash
   gh auth login
   ```

3. **Create repository and push:**
   ```bash
   gh repo create garden-lifecycle-tracker --public --source=. --remote=origin --push
   ```

## After Creating the Repository

Your repository URL will be:
```
https://github.com/YOUR_USERNAME/garden-lifecycle-tracker
```

Update the README.md clone URL if needed:
```bash
git clone https://github.com/YOUR_USERNAME/garden-lifecycle-tracker.git
```

## Repository Structure

The repository includes:
- ✅ Complete ASP.NET Core MVC solution
- ✅ SQLite database with migrations
- ✅ Seed data for crops (Spinach, Carrot, Tomato)
- ✅ Finite State Machine implementation
- ✅ MVC controllers and views
- ✅ Comprehensive README.md
- ✅ Proper .gitignore
- ✅ 2 commits with complete project history

## Quick Test

To verify everything works after cloning:

```bash
# Clone your repo
git clone https://github.com/YOUR_USERNAME/garden-lifecycle-tracker.git
cd garden-lifecycle-tracker

# Restore packages
dotnet restore

# Apply migrations
dotnet ef database update --project GardenTracker.Infrastructure --startup-project GardenTracker.Web

# Run the application
dotnet run --project GardenTracker.Web

# Open browser to:
http://localhost:5140
```

The application is now running and the database has been seeded with sample crops!
