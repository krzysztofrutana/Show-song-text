<!--
*** Thanks for checking out this README Template. If you have a suggestion that would
*** make this better, please fork the repo and create a pull request or simply open
*** an issue with the tag "enhancement".
*** Thanks again! Now go create something AMAZING! :D
***
***
***
*** To avoid retyping too much info. Do a search and replace for the following:
*** github_username, repo_name, twitter_handle, email
-->





<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->

<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/krzysztofrutana/Show-song-text">
    <img src="Show%20song%20text/Show%20song%20text/Resources/Icons/logo.png" alt="Logo" width="200" height="200">
  </a>

  <h3 align="center">Singer's assistant</h3>

  <p align="center">
    My second android app wrote in c# with use Xamarin.Forms.
The application allows you to add songs and create songs playlist. You can get text from tekstowo.pl to current songs (searching by title or artist name or both). 
From playlist view you can run presentation of songs text by order in playlist. Text size is set to 20, but number of lines is calculated for device. 
You can use one devices as server to set text presentation and changing pages and other devices in band can connect to server (server client socket solution) 
and text will automaticaly send to clients. Any clients will see the same text in the same time like server device.

<!-- TABLE OF CONTENTS -->
## Table of Contents

* [About the Project](#about-the-project)
  * [Built With](#built-with)
* [Plan for the future](#plan-for-the-future)
* [Getting Started](#getting-started)
* [Installation](#installation)
* [Contributing](#contributing)
* [Contact](#contact)
* [Thanks](#thanks)



<!-- ABOUT THE PROJECT -->
## About The Project

<p align="center">
  <a href="https://github.com/krzysztofrutana/Show-song-text">
    <img src="https://i.ibb.co/h9Bvn0P/Menu.png" width="400">
    <img src="https://i.ibb.co/pf4PQdn/Song-List2.png" width="400">
    <img src="https://i.ibb.co/qFn6y9V/SongList.png" width="400">  
    <img src="https://i.ibb.co/v3BNvcG/AddSong5.png" width="400">   
  </a>


The main window of this application is the list of added songs. From there, you can select songs and add them to your new playlist. In addition, when you click on a song, 
you can edit the song along with the ability to delete the song or change information such as artist, title or lyrics (looks exactly like add song window) and see which playlist 
it is added to.
<br />
<br />
<br />
<br /><br />

<p align="center">
  <a href="https://github.com/krzysztofrutana/Show-song-text">
    <img src="https://i.ibb.co/6JC0d7S/AddSong1.png" width="400">
    <img src="https://i.ibb.co/yPJk70X/AddSong2.png" width="400">
    <img src="https://i.ibb.co/dKvrVBy/AddSong3.png" width="400">   
    <img src="https://i.ibb.co/hDNHqg1/AddSong4.png" width="400">   
  </a>
  
  From this window possible is looking for song text by only artist, only title or both. Text come from output html code from tekstowo.pl. To this solution I use HTMLAgilityPack.
  <br />
<br />
<br />
<br /><br />

<p align="center">
  <a href="https://github.com/krzysztofrutana/Show-song-text">
    <img src="https://i.ibb.co/cyPD8wZ/Connection-Server1.png" width="400">
    <img src="https://i.ibb.co/Vphsq6c/Connection-Server2.png" width="400">
    <img src="https://i.ibb.co/JsJ97gD/Connection-Client1.png" width="400">   
    <img src="https://i.ibb.co/ccnXbND/Connection-Client2.png" width="400">   
  </a>
  
  From connection settings window you can start server (IP is getting from connected network, so you can also run hotspot, 
  connect other devices to server devices and this will be work) on custom port (default is 11000). After start server this window inform about count of connected client. 
  From client page of this window you can write IP and port and connect to server. After connect from this page you can run presentation mode to getting text from server device.
  <br />
<br />
<br />
<br /><br />

<p align="center">
  <a href="https://github.com/krzysztofrutana/Show-song-text">
    <img src="https://i.ibb.co/r0MzS65/Playlist-List.png" width="400">
    <img src="https://i.ibb.co/LgwxQQ7/Playlist-Detail1.png" width="400">
    <img src="https://i.ibb.co/RPN3J3F/Playlist-Detail2.png" width="400">   
    <img src="https://i.ibb.co/gZnTgb0/Presentation.png" width="400">   
  </a>
  
  List of playlist show all created playlist. After click are show detail information about playlist with list of song. From there you can edit name, add or remove
  songs, change order of current song or start text presentation. Edit options show after click on edit button. When text presentation start, if current device is server, 
  automaticly start sending songs text to clients devices.
  <br />
  
<!--Built With -->
### Built With

* [Microsoft Visual Studio](https://visualstudio.microsoft.com/pl/)
* [Xamarin.Forms](https://dotnet.microsoft.com/apps/xamarin/xamarin-forms)
* [Html Agility Pack](https://html-agility-pack.net/)
* [SQLite.NET](https://www.nuget.org/packages/sqlite-net-pcl/)

<!--Plan for the future -->
## Plan for the future
* Create backup options with backup file compatibility with my desktop application. 
* Test all functionality and fix bugs.  Necessary is tests on more devices with different screen size and resolution to check method of fit text of song in presentation mode.

<!-- GETTING STARTED -->
## Getting Started

Add solution to Microsoft Visual Studio. 

## Installation

For now APK file isn't avaliable. 


<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/github_username/repo_name/issues) for a list of proposed features (and known issues).



<!-- CONTRIBUTING -->
## Contributing

Any contributions are welcome, you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
<!--## License-->

<!-- CONTACT -->
## Contact

Krzysztof Rutana - krzysztofrutana@wp.pl

Project Link: [https://github.com/krzysztofrutana/Przypominajka](https://github.com/krzysztofrutana/Przypominajka)

<!-- THANKS -->
## Thanks

Elżbieta Styrkowicz for creating a logo and help with design and color.

<!-- ACKNOWLEDGEMENTS -->
<!-- ## Acknowledgements--> 
<!--* []() -->

