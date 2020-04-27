### Entity Framework Core Scaffolding with Handlebars

# Contributing

_Contributions from the community are welcome!_

## Pull Request Process

> For an overview of the pull request process, watch this [online tutorial](https://youtu.be/gxhbH9fzTDo).

### Fork, Branch, Test

1. Before creating a pull request, first create an [issue](https://guides.github.com/features/issues/) to discuss the contribution you would like to make.
2. [Fork](https://guides.github.com/activities/forking/) this repository.
3. [Clone](https://help.github.com/en/github/creating-cloning-and-archiving-repositories/cloning-a-repository) your **fork** (not the original repository).
4. Create a [branch](https://www.atlassian.com/git/tutorials/using-branches).
   - [Optional] Prefix your branch name with `@fix/` or `@feature/`
    ```
    git checkout master
    git checkout -b @fix/my-bug-fix
    ```
5. If possible, write a _failing_ unit test.
   - The easiest way is to add a test to `HbsCSharpScaffoldingGeneratorTests` in the *test/Scaffolding.Handlebars.Tests* project.
6. Run the scaffolding command from a command prompt at the *sample/ScaffoldingSample* project.
   - Connect to `(localdb)\MSSQLLocalDB` and create a database named `NorthwindSlim`.
   - Download [northwind-slim](http://bit.ly/northwind-slim), unzip and run `NorthwindSlim.sql`.
   - Install the latest [.NET Core SDK](https://dotnet.microsoft.com/download).
   - Install the global `dotnet ef` tool.
   ```
   dotnet tool install --global dotnet-ef --version 3.1.0-*
   ```
   - Run the EF Core scaffolding command.
   ```
   dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f --context-dir Contexts
   ```
   - Observe generated context and model classes.
7. Implement your proposed code changes.
8. Build the solution.
   ```
   dotnet build
   ```
9. Run unit tests.
   - If you wrote a failing test, ensure that the test now passes.
    ```
    dotnet test
    ```
10. Re-run the EF Core scaffolding command.
    - Ensure that context and model classes are generated as expected.

### Stage, Commit, Push

11. [Stage](https://www.atlassian.com/git/tutorials/saving-changes) and [commit](https://www.atlassian.com/git/tutorials/saving-changes/git-commit) your changes.
    - Add as many commits as you like. (Later you will squash these into a single commit.)
    ```
    git add .
    git commit -m "Commit message"
    ```
12. Push your branch.
    ```
    git push -u origin @fix/my-bug-fix
    ```

### Create Pull Request

13. Create a [pull request](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/about-pull-requests).
    - Include a general description of your proposed changes.
    - Put `Closes #XXXX` in your comment. This will auto-close the issue that your PR fixes.
14. After the repo owner reviews your PR, implement any requested changes.
    ```
    git add .
    git commit -m "Commit message"
    git push
    ```
15. If your PR is open for some time, you will need to [sync your fork](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/syncing-a-fork) with the original repository.
    - Add an _upstream_ that points to the original repository.
    ```
    git remote add upstream https://github.com/TrackableEntities/EntityFrameworkCore.Scaffolding.Handlebars.git
    ```
    - Pull upstream commits to `master` and rebase local commits on top.
    ```
    git pull --rebase upstream master
    ```
    - Resolve conflicts if necessary. Using Visual Studio Code, you can select `Accept Current Change`, `Accept Incoming Change`, A`ccept Incoming Change`, `Accept Both Changes`, or `Compare Changes`.
    - Once you resolve conflicts in a file, make sure to _save_ the file. Then _stage_ changes and continue rebase.
    ```
    git add .
    git rebase --continue
    ```
    - Because `rebase` changes the commit id, you need to push changes to your forked repo using the `--force` flag.
    ```
    git push --force
    ```

### Squash Commits

16. After the reviewer accepts your PR, [squash](http://gitready.com/advanced/2009/02/10/squashing-commits-with-rebase.html) your commits into a single commit.
    - If you like install [Visual Studio Code](https://code.visualstudio.com/) and make it your default Git editor.
    ```
    git config --global core.editor "code --wait"
    ```
    - Configure git log format.
    ```
    git config --global alias.lg "log --color --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Creset %s %Cgreen(%cr) %C(bold blue)<%an>%Creset' --abbrev-commit"
    ```
    - Determine the *number of commits* you need to squash by looking at the git log. Type `q` to exit the log.
    ```
    git lg
    ```
    - Use interactive rebase to squash all your commits into one commit, specifying after `HEAD~` the number of commits you wish to squash. During this process you will set the commit message.
    ```
    git rebase -i HEAD~4
    ```
17. Push your squashed commit.
    - Because rebase changes commits, you will need to add the `--force` flag when pushing your commits.
    ```
    git push --force
    ```
18. Once you your PR has been accepted and merged, you can _pull_ `master` and _delete_ your local branch.
    ```
    git checkout master
    git pull
    git branch -d @fix/my-bug-fix
    ```
