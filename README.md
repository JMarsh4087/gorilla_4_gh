# 🦍 Gorilla for Grasshopper

**Gorilla** is a collection of interactive and data-centric components for Grasshopper. It includes custom UI widgets like clickable grid selectors, as well as utility nodes designed to enhance data manipulation and workflow clarity.

---

## 📦 Features

- **🧩 Gorilla Grid Selector**: A clickable 2D grid component that allows you to select cells interactively on the canvas.
- **🔗 Gorilla Ordered Merge**: Merge multiple lists while preserving their original data structure and index order.
- *(More nodes coming soon!)*

---

## 📁 Installation

1. Download the latest `Gorilla.gha` file from the [Releases](https://github.com/YOUR-USERNAME/gorilla_4_gh/releases) page.
2. Place it in your **Grasshopper Libraries folder**: %AppData%\Grasshopper\Libraries\
3. Restart Rhino + Grasshopper.
4. You’ll now find Gorilla components under the **Params > Util** tab (or search "Gorilla").

---

## 📄 Included Files

Inside the `/release` folder:

- `Gorilla.gha` — The compiled plugin file
- `Gorilla_Demo.gh` — An example Grasshopper file demonstrating the Grid Selector
- `README.txt` — Basic usage and install instructions (optional)

---

## 🧪 Example Usage

Open `Gorilla_Demo.gh` in Grasshopper to test:
- Click on the grid UI to select cells
- Watch outputs update in real time
- Try using the Ordered Merge node to sort structured lists

---

## 🛠️ Building from Source

To build Gorilla from source:

1. Clone this repo:
```bash
git clone https://github.com/YOUR-USERNAME/gorilla_4_gh.git

Open Gorilla.sln in Visual Studio.

Build the project (targets .NET Framework 4.8 by default).

Output .gha file will appear in /bin/Release/.

