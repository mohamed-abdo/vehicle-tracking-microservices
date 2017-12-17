# vehicle-tracking-microservices
a vehicle tracking system (microservices architecture [CI/CD])

# Epic story
A vehicles company have a number of connected vehicles that belongs to a number of customers.
- They would like be able to view the status of the connection among these vehicles on a monitoring display.
- The vehicles send the status of the connection one time per minute; no request from the vehicle means no connection.
- The company would like to create a data store that keeps these vehicles with their status and the customers who own them, as well as a web-based that displays the status.
- dessign and architecture should follows microservices, CI/CD principals, as well API/ cloud first.

# Domain 
Domain defintion following swagger (YAML) format
```
vehicle:
    type: object
    properties:
      correlatId:
        type: string
        example: 0729a580-2240-11e6-9eb5-0002a5d5c51b
      status:
        type: string
        enum:
        - active
        - inactvie
        - warning
        - danger
        - information
      detailes:
        type: string
        example: an issue with the engine.
          
  vehicleResponse:
    type: object
    properties:
      response:
        $ref: '#/definitions/responseModel'
      data:
        type: string
        enum:
        - ok
        - failed
        description: ok in case of successful, otherwise failed.
  
  vehicleStatusResponse:
    type: object
    properties:
      vehicle:
        type: string
        example: vkfbvfbd03423
      owner:
        type: string
        example: mohamed abdo
      vehicleRef:
        type: string
        example: 0729a580-2240-11e6-9eb5-0002a5d5c51b
      ownerRef:
        type: string
        example: 0729a580-2240-11e6-9eb5-0002a5d5c51b
      status:
        type: string
        example: warning
      lastUpdate:
        type: string
        format: date-time
        example: 2017-04-12T23:20:50.52Z
      message:
        type: string
        example: {engine:.97, oil:344000}
  
  responseModel:
    type: object
    properties:
      executionId:
        type: string
        example: 0729a580-2240-11e6-9eb5-0002a5d5c51b
      correlationId:
        type: string
        example: 0729a580-2240-11e6-9eb5-0002a5d5c51b
      issucceed:
        type: boolean
      code:
        type: integer
        example: 1034034
        description: response code, 0 in case of succeed, else positive integer.
      hint:
        type: object
        properties:
          action:
            type: string
            enum:
            - ok
            - retry
            - correct-inputs
            - in-maintenance
            - system-error
            - obsoleted
      messages:
        type: string
        example: vehicle Id is not registered
```   
https://app.swaggerhub.com/domains/soft-ideas/autonomous-vehicle/1.0

# Features scope

# Vehicles ping
As a vehicle (IoT device), device shall be able to submit it's status over http for a certain end-point, along with http header header   contains authorization key, on the request body optionaly, device can submit more details about the status, witch correlation Id for      this status to support future device features scaling.
  # Behaviors
  - Device decicdes when to call the end-point.
  - Device doesn't have further functionlity to act according to end-point response.
  - Authourzation token verfication is out of scope for the current implementation.
  # API contract
    swagger API documentation 
https://app.swaggerhub.com/apis/soft-ideas/vehicle-tracking/1.0.0

# Vehicle tracking
As a vehicle tracker role owner, i would like to view vehicles along with their status, as well filter resuls by customer, and vehicle status; an addition to will able to find detailed link to view customer page, or vehicle page.
  # Behaviors
  - Page will auto refresh each minute.
  - link for the customer, and vehicle page is out of scope for the current implementation.
  - Authourzation token verfication is out of scope for the current implementation.
  # API contract
    swagger API documentation 
https://app.swaggerhub.com/apis/soft-ideas/vehicle-tracking/1.0.0
    
