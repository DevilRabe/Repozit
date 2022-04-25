<?php
    include("..\/Controller/dbconnect.php");

    $login = $_POST['Login'];
    $password = $_POST['Password'];

    $sql = "SELECT * FROM accounts WHERE user_login = '$login'";
    $result = mysqli_query($mysqli, $sql);

    $password = md5($password);
    $RandomKey = mt_rand(1,50);
    $Ukey = hash_hmac('sha512', $value, $RandomKey);
    $Banned = 0;

    if(preg_match("#^[a-z0-9]+$#i", $login))
    {
        if(mysqli_num_rows($result) == 0)
        {
            $mysqli->query ("INSERT INTO accounts(user_login, user_password, user_key, user_banned) VALUES ('$login', '$password', '$Ukey', '$Banned')");
            echo json_encode(array('return' => '0'));
        }
        else {echo json_encode(array('return' => '1'));}
    }
    else{echo json_encode(array('return' => $login));}

    $mysqli->close();

?>