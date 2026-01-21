# REUSE Headers

These are open source files for you to setup REUSE headers on your own repo, if you or anyone who wants to fuck with you decides to do this bullshit.

It's quite simple, here's the steps.

## Step 1: Realizing that you did this because you care about self-preservation, not because of some moral obligation.

## Step 2: Run init_reuse_headers.py
`init_reuse_headers.py` has a couple of functions.

`--dry-run` let's you know that everything is going to be okay. (You are checking to make sure your settings are correct)

`--workers=4` let's you set the amount of workers you want to use for running this program.

Both options can be used together. If you want to modify your files, I recommend closing whatever IDE you have open, and running this through the command line.

## Step 3: Adding the changes to Git
`git add .` and `git commit` are all you need to now have REUSE headers on your repo.

## Step 4: Make sure your Github Actions file works
After you either cherry-pick this PR or copy and paste it, make a test 