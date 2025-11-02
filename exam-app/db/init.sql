-- Database initialization script

CREATE TABLE IF NOT EXISTS users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  email VARCHAR(255) NOT NULL UNIQUE,
  password VARCHAR(255) NOT NULL,
  name VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS exams (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  name VARCHAR(255) NOT NULL,
  score INT NOT NULL,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Sample users (passwords in cleartext as requested)
INSERT INTO users (email, password, name) VALUES
('alice@example.com', 'alicepass', 'Alice Johnson'),
('bob@example.com', 'bobpass', 'Bob Smith'),
('charlie@example.com', 'charliepass', 'Charlie Brown');

-- Sample exams for Alice (id = 1)
INSERT INTO exams (user_id, name, score) VALUES
(1, 'Mathematics 101', 82),
(1, 'History 201', 74),
(1, 'Physics 301', 90),
(1, 'Chemistry 101', 68);

-- Sample exams for Bob (id = 2)
INSERT INTO exams (user_id, name, score) VALUES
(2, 'Mathematics 101', 60),
(2, 'Chemistry 101', 77),
(2, 'Biology 201', 85);

-- Sample exams for Charlie (id = 3)
INSERT INTO exams (user_id, name, score) VALUES
(3, 'Physics 301', 95),
(3, 'Mathematics 101', 72),
(3, 'Computer Science 101', 88);
