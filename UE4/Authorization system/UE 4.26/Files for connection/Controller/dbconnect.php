<?php
    $DB_host = "HOSTNAME";
    $DB_user = "USERNAME";
    $DB_pass = "USERPASSWORD";
    $DB_name = "DBNAME";

    $mysqli = new MySQLi($DB_host, $DB_user, $DB_pass, $DB_name);

    if($mysqli->connect_errno)
    {
        die("ERROR: -> ".$mysqli->connect_error);
    }
    
?>