# vehicle-tracking-microservices
a vehicle tracking system (microservices architecture)

# Epic story
A vehicles company have a number of connected vehicles that belongs to a number of customers.
They have a need to be able to view the status of the connection among these vehicles on a monitoring display.
The vehicles send the status of the connection one time per minute; no request from the vehicle means no connection.
The company would like to create a data store that keeps these vehicles with their status and the customers who own them, as well as a web-based that displays the status.

# Features scope

# Vehicles ping status

As a vehicle (IoT device), device shall be able to submit it's status over http for a certain end-point, along with http header header   contains authorization key, on the request body optionaly, device can submit more details about the status, witch correlation Id for      this status to support future device features scaling.

  # Behaviors
  - Device decicdes when to call the end-point.
  - Device doesn't have further functionlity to act according to end-point response.
  - Authourzation token verfication is out of scope for the current implementation.
  
  # API contract
    https://app.swaggerhub.com/apis/mohamed-abdo/vehicle-status/1.0.0#/vehicle/ping
  
