# AdditiveSceneGroups

A collection of tools used to facilitate additive scene workflows in Unity.

## Features
* Combine multiple scenes into a group to be loaded additively in Unity
* Auto-resolve scene groups that share the same scenes
* Editor tool:
  * Save scene groups in the editor and load them later with a click of a button
  * Sort through groups for quick access
* Basic hooking with ECS

## Motivation

When it comes to working solo on a project, it is very easy to dump every 
game object and prefab into one scene and just make it work. Sometimes you 
might need to add a new scene for a new level, but at any time in the game,
there is generally one scene active.

However, when working collaborately, having single scenes leads to problems:
* Multiple people working in parallel on the same scenes
* Some may be inexperienced and can mess up the entire scene by accident
* More time is spent resolving conflicts instead of working on the game

In an additive scene workflow, the monolithic scene can be split into 
multiple smaller scenes, each with a significant purpose. For example, a game
 can be composed of `{Logic, UI, Environment }` scenes, where each team member can work separately.  At the end of the day, every scene can be loaded simultaneously to give the illusion of a singular scene.


## Requirements
Unity 2019.1 or newer.  Older versions *can* work, but the scripting backend must be
set to `.NET 4.x Equivalent`.


## Setup
Download/clone the repo or setup locally as a submodule to a unity project

## Runtime Scripts
  * _SceneLoader_
    * Manages scene group loading 
    * Is a _ScriptableObject_ that lives in the project folder, so it can be treated as a draggable asset
  * _SceneManifest_
    * Stores scene groups and their states
    * Groups can be edited with a custom editor window
  * _SceneBootstrapper_
    * MonoBehaviour wrapper to the scene loader
    * Has basic functionality such as autoload group on start
    * Can load groups directly from the _SceneManifest_

