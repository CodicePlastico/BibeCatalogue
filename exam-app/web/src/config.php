<?php
// Database configuration
$DB_HOST = 'db';
$DB_NAME = 'examdb';
$DB_USER = 'examuser';
$DB_PASS = 'exampass';

// Function to create database connection with retry logic
function createDatabaseConnection($host, $user, $pass, $dbname, $maxRetries = 10) {
    $retries = 0;
    
    while ($retries < $maxRetries) {
        try {
            $mysqli = new mysqli($host, $user, $pass, $dbname);
            
            if ($mysqli->connect_errno) {
                throw new Exception("Connection failed: " . $mysqli->connect_error);
            }
            
            // Set charset to handle special characters properly
            $mysqli->set_charset("utf8");
            return $mysqli;
            
        } catch (Exception $e) {
            $retries++;
            if ($retries >= $maxRetries) {
                die("Failed to connect to database after $maxRetries attempts: " . $e->getMessage());
            }
            
            // Wait before retrying (exponential backoff)
            sleep(min(pow(2, $retries), 10));
        }
    }
}

// Create the database connection
$mysqli = createDatabaseConnection($DB_HOST, $DB_USER, $DB_PASS, $DB_NAME);
?>
