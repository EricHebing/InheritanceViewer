# InheritanceViewer
A Visual Studio extension (VSIX) to  graphically represent C++ Inheritances

# Hot to Use
![image](https://github.com/EricHebing/InheritanceViewer/assets/78701937/25211a10-6153-4900-b971-1d82a7eefdd2)

Rightclick anywhere in a header file and select "Inheritance Graph" as shown.
The extension parses the header file for all C++ class declarations and their inheritance information.
All header and .cpp files of the project the header file belongs to are searched and parsed for their class declarations and inheritance information as well.
A Inheritance Graph is build up and written to a temporary .dgml file and shown afterwards.

# Prerequisites
To properly show the generated dgml-file in Visual Studio the DGML-Editor component has to be installed.
![image](https://github.com/EricHebing/InheritanceViewer/assets/78701937/ea9d56f9-5de0-4521-b823-d3d76dbce022)


If this component is not installed a message is shown in the dgml-file as well.

