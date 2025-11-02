<?php
session_start();
require_once 'config.php';

/*
if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    header('Location: index.php');
    exit;
}

$email = isset($_POST['email']) ? trim($_POST['email']) : '';
$password = isset($_POST['password']) ? $_POST['password'] : '';
*/

$email = isset($_POST['email']) ? trim($_POST['email']) : (isset($_GET['email']) ? trim($_GET['email']) : '');
$password = isset($_POST['password']) ? $_POST['password'] : (isset($_GET['password']) ? trim($_GET['password']) : '');

// Basic validation
if (empty($email) || empty($password)) {
    $_SESSION['error'] = 'Please enter both email and password.';
    header('Location: index.php');
    exit;
}

/* No SQL Injection
// Fetch user by email
$stmt = $mysqli->prepare("SELECT id, name, password FROM users WHERE email = ?");
$stmt->bind_param('s', $email);
$stmt->execute();
$result = $stmt->get_result();
$user = $result->fetch_assoc();
$stmt->close();

// Verify credentials (plaintext comparison as requested)
if ($user && $password === $user['password']) {
    // Login successful
    $_SESSION['user_id'] = $user['id'];
    $_SESSION['user_name'] = $user['name'];
    $_SESSION['user_email'] = $email;
    
    header('Location: exams.php');
    exit;
} else {
    $_SESSION['error'] = 'Invalid email or password. Please try again.';
    header('Location: index.php');
    exit;
}
*/

$sql = "SELECT id, name, password FROM users WHERE email = '" . $email . "' AND password = '" . $password . "'";
$result = mysqli_query($mysqli, $sql);
if (mysqli_num_rows($result) > 0) {
    $row = mysqli_fetch_assoc($result);
    // Login successful
    $_SESSION['user_id'] = $row['id'];
    $_SESSION['user_name'] = $row['name'];
    $_SESSION['user_email'] = $email;
    
    header('Location: exams.php');
    exit;
} else {
    $_SESSION['error'] = 'Invalid email or password. Please try again.';
    header('Location: index.php');
    exit;
}

?>
