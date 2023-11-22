# InheritanceViewer
A Visual Studio extension (VSIX) to  graphically represent C++ Inheritances like Doxygen

![image](https://github.com/EricHebing/InheritanceViewer/assets/78701937/a5cf25b4-3548-4f25-a76e-cfb8fceb114c)



# How to Use
![image](https://github.com/EricHebing/InheritanceViewer/assets/78701937/b62baa0c-a5c2-4e72-84a9-b1780ad2f9fb)


Rightclick anywhere in a header file and select "Inheritance Graph" as shown.
The extension parses the header file for all C++ class declarations and their inheritance information.
All header and .cpp files of the project the header file belongs to are searched and parsed for their class declarations and inheritance information as well.
A Inheritance Graph is build up and written to a temporary .dgml file and shown afterwards.

# Prerequisites
To properly show the generated dgml-file in Visual Studio the DGML-Editor component has to be installed.
![image](https://github.com/EricHebing/InheritanceViewer/assets/78701937/668cc1b8-80d0-4339-b3b8-bd41b06a0865)



If this component is not installed a message is shown in the dgml-file as well.

