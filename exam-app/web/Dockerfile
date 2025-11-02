FROM php:8.1-apache

# Install mysqli extension
RUN docker-php-ext-install mysqli

# Copy application files
COPY src/ /var/www/html/

# Set proper permissions
RUN chown -R www-data:www-data /var/www/html

EXPOSE 80
