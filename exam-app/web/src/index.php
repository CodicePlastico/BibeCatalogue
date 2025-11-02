<?php
session_start();

// Redirect if already logged in
if (isset($_SESSION['user_id'])) {
    header('Location: exams.php');
    exit;
}

// Get error message if any
$error = '';
if (isset($_SESSION['error'])) {
    $error = $_SESSION['error'];
    unset($_SESSION['error']);
}
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Exam Portal - Login</title>
    <link rel="stylesheet" href="style.css">
</head>
<body>
    <div class="container login-container">
        <div class="header">
            <h1>📚 Exam Portal</h1>
        </div>
        
        <div class="content">
            <?php if ($error): ?>
                <div class="alert alert-error">
                    <?php echo htmlspecialchars($error, ENT_QUOTES); ?>
                </div>
            <?php endif; ?>

            <form method="post" action="login.php">
                <div class="form-group">
                    <label for="email">Email Address</label>
                    <input type="email" id="email" name="email" required placeholder="Enter your email">
                </div>

                <div class="form-group">
                    <label for="password">Password</label>
                    <input type="password" id="password" name="password" required placeholder="Enter your password">
                </div>

                <button type="submit" class="btn btn-primary" style="width: 100%;">
                    Sign In
                </button>
            </form>

            <div class="sample-accounts">
                <h3>Sample Test Accounts</h3>
                <ul>
                    <li><strong>Alice:</strong> alice@example.com / alicepass</li>
                    <!--<li><strong>Bob:</strong> bob@example.com / bobpass</li>
                    <li><strong>Charlie:</strong> charlie@example.com / charliepass</li>-->
                </ul>
            </div>
        </div>
    </div>
</body>
</html>
