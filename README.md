MD5-Update.

Tool for keep update your application, update all files in your application directory with (Libraries, Executables, etc). All file comparisons make with MD5 hash. No need configure FTP, Version App, only need website or public server with PHP.

For use the library.
Your need webserver with PHP for publish the files you want update on in update folder, in this folder your add index.php for make JSON with list of all files and directories in update folder with your md5 hash.

For update your app:
When your app start call the library will return bool. library get JSON file list from update folder, if any file doesn't exist o have distinct MD5 hash, will be added to download list, return true is download list not empy and false if is empty.
