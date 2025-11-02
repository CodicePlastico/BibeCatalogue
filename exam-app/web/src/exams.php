<?php
session_start();

// Check if user is logged in
if (!isset($_SESSION['user_id'])) {
    header('Location: index.php');
    exit;
}

require_once 'config.php';

$user_id = $_SESSION['user_id'];
$user_name = $_SESSION['user_name'];
$user_email = $_SESSION['user_email'];

// Fetch exams for this user
$stmt = $mysqli->prepare("SELECT name, score FROM exams WHERE user_id = ? ORDER BY name");
$stmt->bind_param('i', $user_id);
$stmt->execute();
$result = $stmt->get_result();
$exams = $result->fetch_all(MYSQLI_ASSOC);
$stmt->close();

// Calculate stats
$total_exams = count($exams);
$passed_exams = 0;
$total_score = 0;

foreach ($exams as $exam) {
    if ($exam['score'] >= 75) {
        $passed_exams++;
    }
    $total_score += $exam['score'];
}

$average_score = $total_exams > 0 ? round($total_score / $total_exams, 1) : 0;
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>My Exams - Exam Portal</title>
    <link rel="stylesheet" href="style.css">
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>📊 My Exam Results</h1>
        </div>

        <div class="content">
            <div class="top-bar">
                <div class="user-info">
                    <h2>Welcome, <?php echo htmlspecialchars($user_name, ENT_QUOTES); ?>!</h2>
                    <p><?php echo htmlspecialchars($user_email, ENT_QUOTES); ?></p>
                </div>
                <form method="post" action="logout.php" style="display: inline;">
                    <button type="submit" class="btn btn-secondary">Logout</button>
                </form>
            </div>

            <?php if ($total_exams > 0): ?>
                <!-- Stats Summary -->
                <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 20px; margin-bottom: 30px;">
                    <div style="background: #f8f9fa; padding: 20px; border-radius: 8px; text-align: center;">
                        <div style="font-size: 24px; font-weight: bold; color: #333;"><?php echo $total_exams; ?></div>
                        <div style="color: #666;">Total Exams</div>
                    </div>
                    <div style="background: #d4edda; padding: 20px; border-radius: 8px; text-align: center;">
                        <div style="font-size: 24px; font-weight: bold; color: #155724;"><?php echo $passed_exams; ?></div>
                        <div style="color: #155724;">Passed</div>
                    </div>
                    <div style="background: #f8d7da; padding: 20px; border-radius: 8px; text-align: center;">
                        <div style="font-size: 24px; font-weight: bold; color: #721c24;"><?php echo $total_exams - $passed_exams; ?></div>
                        <div style="color: #721c24;">Failed</div>
                    </div>
                    <div style="background: #e2e3e5; padding: 20px; border-radius: 8px; text-align: center;">
                        <div style="font-size: 24px; font-weight: bold; color: #383d41;"><?php echo $average_score; ?>%</div>
                        <div style="color: #383d41;">Average</div>
                    </div>
                </div>

                <!-- Exams Table -->
                <div class="table-container">
                    <table>
                        <thead>
                            <tr>
                                <th>Exam Name</th>
                                <th>Score</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            <?php foreach ($exams as $exam): 
                                $score = (int)$exam['score'];
                                $passed = $score >= 75;
                            ?>
                            <tr>
                                <td><?php echo htmlspecialchars($exam['name'], ENT_QUOTES); ?></td>
                                <td>
                                    <span class="score"><?php echo $score; ?>%</span>
                                </td>
                                <td>
                                    <span class="<?php echo $passed ? 'status-passed' : 'status-failed'; ?>">
                                        <?php echo $passed ? '✅ Passed' : '❌ Failed'; ?>
                                    </span>
                                </td>
                            </tr>
                            <?php endforeach; ?>
                        </tbody>
                    </table>
                </div>

                <div style="margin-top: 20px; padding: 15px; background: #e9ecef; border-radius: 8px; font-size: 14px; color: #6c757d;">
                    <strong>Note:</strong> A minimum score of 75% is required to pass an exam.
                </div>

            <?php else: ?>
                <div class="empty-state">
                    <h3>No Exams Found</h3>
                    <p>You haven't taken any exams yet.</p>
                </div>
            <?php endif; ?>
        </div>
    </div>
</body>
</html>
