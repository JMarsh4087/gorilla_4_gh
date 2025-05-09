# 🦍 GORILLA 4 GRASSHOPPER 🦍

**Gorilla** is a collection of interactive and data-centric components for Grasshopper. It includes custom UI widgets like clickable grid selectors, as well as utility nodes designed to enhance data manipulation and workflow clarity.

---

## 📦 Features:

- **🦍 Gorilla Grid**: A clickable 2D grid component that allows you to select cells interactively on the canvas for use with UI tools that ask for column and row integers (such as Human UI).
- **🦍 Gorilla Ordered Merge**: Merge multiple lists while preserving their original data structure and index order. Option to rebuild tree structure.
- **🦍 Gorilla Line Flip By Boolean**: Given true/false pattern, plug in a set of lines to flip accordingly.
- **🦍 Gorilla Memory**: Internalizes data to keep it persistent between Rhino sessions. (Open to suggestions on this one.)
- **🦍 Gorilla Sort Points Weighted by XYZ**: Similar to Kingfisher sort points by x, y, z, but allows for all coordinates to be considered together and given weights.
- **🦍 Gorilla Point Sort Priority**: Meant to work with Gorilla Sort Points Weighted by XYZ. Provides dropdowns for the built-in priorities (Ignore, High, Medium, Low).
- *(More nodes coming soon!)*

---

## 📁 Installation:

1. Download the latest `Gorilla.gha` file from the [Releases](https://github.com/YOUR-USERNAME/gorilla_4_gh/releases) page. (Contact me if you want me to create a new release.)
2. Place it in your **Grasshopper Libraries folder**: %AppData%\Grasshopper\Libraries\
3. Restart Rhino + Grasshopper.
4. You’ll now find a Gorilla tab in Grasshopper.

---

## 📄 Included Files:

Inside the `/release` folder:

- `Gorilla.gha` — The compiled plugin file
- `Gorilla_Example_v01.gh` — An example Grasshopper file demonstrating the Grid Selector and Ordered Merge
- `README.txt` — Basic usage and install instructions (optional copy of the file you are currently reading)

---

## 🧪 Example Usage:

Open `Gorilla_Example_v01.gh` in Grasshopper to test:
- Click on the grid UI to select cells
- Watch outputs update in real time
- Try using the Ordered Merge node to sort structured lists
- Too lazy to keep going. May update later.

---

## 🛠️ Building from Source

To build Gorilla from source:

1. Clone this repo:
```bash
git clone https://github.com/YOUR-USERNAME/gorilla_4_gh.git
```
2. Open Gorilla.sln in Visual Studio.

3. Build the project (targets .NET Framework 4.8 by default).

3. Output .dll file will appear in /bin/Debug/.

4. Rename the .dll to .gha extension.

5. Place the .gha is your Grasshopper Libraries folder and start Rhino/Grasshopper.
