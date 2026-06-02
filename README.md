# 🟡 3D Pac-Man: Interactive Environment & Intelligent Graph System

[![Unity Version](https://img.shields.io/badge/Unity-6%20(URP)-black?logo=unity)](#)
[![C#](https://img.shields.io/badge/Language-C%23-blue)](#)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](#)

A fully playable, autonomous 3D Pac-Man simulation developed as a joint academic project for the **SE3032 (Graphics & Visualization)** and **SE3062 (Intelligent Systems)** modules. 

This project explores the intersection of procedural 3D environment generation, real-time lighting aesthetics, and mathematically rigorous AI pathfinding algorithms ($A^*$, BFS) operating on dynamically changing graph structures.

---

## 🏛 Academic Context
* **Institution:** Sri Lanka Institute of Information Technology (SLIIT)
* **Degree:** BSc (Hons) in Computer Science
* **Academic Level:** Year 3, Semester 1
* **Modules:** SE3032 Graphics and Visualization & SE3062 Intelligent Systems

---

## 👥 Team & Roles

| Student ID | Name | Project Role | Module Focus |
| :--- | :--- | :--- | :--- |
| **IT23750210** | **K.H.G.A. Udaneth** | World Builder & Graph Formulator | GV / IS |
| **IT23645684** | **V.N. Jayasinghe**  | Systems Engineer & Dynamic Adaptation | GV / IS |
| **IT23575608** |   **H.W. Ranwala**   | Core Developer & Primary Search ($A^*$) | GV / IS |
| **IT23575776** |  **M.S.A.O.Kumara**  | Agent Controller & Secondary Search | GV / IS |

---

## 🛠 System Architecture & Contributions

### 1. World Builder & Graph Formulator (IT23750210)
Responsible for the foundational 3D environment (GV) and its translation into a mathematical data structure (IS).

**Graphics & Visualization (GV):**
* **Procedural Maze Generation:** Implemented `MapGenerator.cs` to dynamically generate a 3-level progression system from 2D integer arrays, allowing instant layout alterations without manual prefab placement.
* **ProBuilder Modeling:** Modeled the foundational `Wall_Prototype` using Unity ProBuilder for mathematically clean, grid-aligned geometry.
* **Shader & Lighting Optimization:** Utilized `Unlit/Color` shaders combined with tiled textures (DXT1 compressed) to completely bypass per-fragment lighting calculations.
* **Post-Processing:** Achieved the retro-arcade neon aesthetic highly efficiently via Global Volume Post-Processing (Bloom, Vignette, Color Adjustments).
* **Spatial UI:** Developed a real-time rotating Minimap system utilizing an overhead Orthographic camera and a `256x256` RenderTexture.

**Intelligent Systems (IS):**
* **NavMesh Graph Extraction:** Developed `NavMeshGraphExtractor.cs` to programmatically sample the 3D Unity NavMesh and extract valid traversal nodes.
* **Distance-Based Adjacency:** Engineered an $O(R \times C + N)$ extraction algorithm utilizing distance-based edge validation ($\le 1.5 \times \text{tileSize}$). This solved Unity NavMesh boundary false-negatives, creating a 100% accurate mathematical Adjacency List for the AI agents.

---

### 2. Systems Engineer & Dynamic Adaptation (IT23645684)
<!-- STUDENT 2: Please fill in your specific contributions below -->

**Graphics & Visualization (GV):**
* *[Add your physics, collision, and environmental interaction details here]*
* *[Add any physics optimization techniques (Fixed Timesteps, Rigidbody constraints) here]*

**Intelligent Systems (IS):**
* *[Add how your barricade logic intercepts the game state here]*
* *[Add how you sever graph edges in the adjacency list and trigger recalculations here]*

---

### 3. Core Developer & Primary Search (IT23575608)
<!-- STUDENT 3: Please fill in your specific contributions below -->

**Graphics & Visualization (GV):**
* *[Add details about your custom Blender 3D models here]*
* *[Add details regarding topology, polygon counts, and UV mapping here]*

**Intelligent Systems (IS):**
* *[Add your explanation of the A* search math and heuristic choices here]*
* *[Add details on your algorithmic complexity and Priority Queue implementation here]*

---

### 4. Agent Controller & Secondary Search (IT23575776)
<!-- STUDENT 4: Please fill in your specific contributions below -->

**Graphics & Visualization (GV):**
* *[Add how you handled smooth 3D translations, Lerp/Slerp rotations, and tied animations here]*

**Intelligent Systems (IS):**
* *[Add your secondary search (BFS/UCS) algorithm details here]*
* *[Add how your toggleable Debug Mode visualizer draws the mathematical graph in 3D space here]*

---

## ⚙️ Development Environment & Setup
* **Engine:** Unity 6 (Universal Render Pipeline)
* **Language:** C#
* **Performance:** Developed and highly optimized for modern hardware configurations, including seamless testing on Apple Silicon (M4, 24GB RAM).

**How to Run:**
1. Clone this repository.
2. Open the project in Unity 6.
3. Open the `StartScreen` scene located in `Assets/Scenes/`.
4. Press **Play** to start the simulation.

## 🌿 Git Branching Strategy
To prevent `.unity` scene corruption and YAML merge conflicts, this project strictly adheres to a feature-branching workflow. 
* `main`: The stable, integrated project state.
* `IT23750210_WorldBuilder`: Feature branch for all environment layout, graph extraction, and lighting.
* `[Student_2_Branch]`: Feature branch for dynamic physics.
* `[Student_3_Branch]`: Feature branch for AI heuristics.
* `[Student_4_Branch]`: Feature branch for agent controls.
