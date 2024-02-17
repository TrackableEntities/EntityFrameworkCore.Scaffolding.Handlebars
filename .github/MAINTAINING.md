### Entity Framework Core Scaffolding with Handlebars

# Maintaining

_This document describes tasks for maintaining a GitHub repository and publishing updated packages to NuGet._

### Pull Requests

1. Follow command-line instructions to pull contributor's branch.
   - Copy just the first two lines and paste them into a terminal.
   - This will create a local branch for the PR which you can inspect.
2. Run unit tests to make sure they all pass.
3. Run EF Core scaffolder on the sample project to see if any output files have changed.
4. Inspect proposed changes. If necessary, comment on lines of code in the PR and propose changes.
5. When the PR is ready to merge, you can do it locally or via the GitHub interface.

### Releases

1. Increment the package version number.
   - Append beta1 (or later) to indicate "pre-release" status.
2. Enter the matching version number in the release notes URL.
3. Commit and push the change to the master branch.
4. Create a new release in GitHub
   - Create a tag with "v" prepended to the version number (for example v8.2.1).
   - The title of the release should match the tag you just created.
   - Add release notes that include the issue and pull request numbers included in the release.
   - Paste a link to the NuGet package you will create next.
   - If it is a beta release, set as a pre-release.
   - Publish the release.

### NuGet

1. Set the build configuration to Release.
2. Build the project.
3. Navigate to the bin/release folder.
4. Log into NuGet.
5. Under your login name select Upload Package.
6. Drop the file into the Upload box (or select Browse to locate).
7. Review package details to verify their correctness.
8. Publish the package.

