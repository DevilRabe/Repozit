<?php
    include("..\/Controller/dbconnect.php");

    $login = $_POST['Login'];
    $password = $_POST['Password'];

    $password = md5($password);

    $sql = "SELECT * FROM accounts WHERE user_login = '$login'";
    $result = mysqli_query($mysqli, $sql);

    if(mysqli_num_rows($result) > 0)
    {
        while($row = mysqli_fetch_assoc($result))
        {
            if($password == $row['user_password'])
            {
                if(0 == $row['user_banned'])
                echo json_encode(array('return' => '0'));

                else
                echo json_encode(array('return' => '2'));
            }
            else{echo json_encode(array('return' => '1'));}
        }
    }
    else{echo json_encode(array('return' => '1'));}

?>