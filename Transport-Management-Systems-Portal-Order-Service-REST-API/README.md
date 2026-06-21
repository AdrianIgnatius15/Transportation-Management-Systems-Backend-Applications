## Transport Management Systems Portal {TMS} 📦🚛 (Order Service) REST API

This code represents the business logic and execution of it's code to help customers (business owners) create their shipment orders to be sent to the system.

It will help the carriers to view all orders that may contain the size of the shipments, the type of shipment goods, the value of shipments, the delivery date of the shipments and the destination/shipped address so that they can calculate the rates & make the decision on which type of transportation mode the shipment can be delivered.

## Commands to build the Dockerfile into a 🐳 Docker image

Firstly, this project uses a multi-stage strategy creation of a Docker image. A multi-stage Dockerfile uses multiple `FROM` instructions to create smaller, more secure, and efficient images by separating the build environment from the production runtime. You compile code and install dependencies in an initial heavy stage, then use `COPY --from` to move only the final artifacts to a lightweight, minimal production image.

    1. Run this command below
        `docker build -t transport-management-systems-portal-order-service-rest-api .`
    2. After building the Docker image, then run the command below
        `docker login` --> Ensure you login into your own DockerHub or any other image hub repository.
    3. Once logged in, run this command to tag the locally build Docker image to a tag name which points to your remote image hub repository
        `docker tag transport-management-systems-portal-order-service-rest-api adrianjoseph15/transport-management-systems-portal-order-rest-api:latest`
    4. Then, finally push the Docker image to the remote repository hub which you logged in, this uses DockerHub but feel free to use others
        `docker push adrianjoseph15/transport-management-systems-portal-order-rest-api:latest`

Refer to this video to understand of multi-stage Dockerfile image [https://youtu.be/V0kTEk7YA70?si=x6Y3niUnHi72kC2w](
Docker Multistage builds explained in 8 minutes).

## Creating Keycloak SPI Events Listener for User Registration
After a user has created an account in Keycloak as Keycloak is an Identity Server, the REST API has no way of finding out that the user has created an account. Hence, Keycloak allows developers to create an Event Listener by creating a simple Java SpringBoot application for it and it will then call the exposed endpoint of your backend to pass the new user created data.

(Keycloak SPI Event Listener)[https://dev.to/adwaitthattey/building-an-event-listener-spi-plugin-for-keycloak-2044]

To create and build the Event Listener, simply clone the project, ensure that you have Java & Maven installed and run `mvn clean install`.
Once that's done, head to the generated "target" folder and copy the `sample-event-listener.jar`. This build Event Listener will be then used to hook to our Keycloak Docker Image from Quarkus as our "volume" to provide to. The code for the "volume" in the dockec-compose file is as below:

```yml
keycloak:
    image: quay.io/keycloak/keycloak:latest
    container_name: keycloak
    ports:
      - "127.0.0.1:8080:8080"
    environment:
      KEYCLOAK_ADMIN: admin
      KEYCLOAK_ADMIN_PASSWORD: admin
      KC_SPI_EVENTS_LISTENER_HTTP_SERVER_URI: http://host.docker.internal:5000/api/client/user-registration-sync
      KC_SPI_EVENTS_LISTENER_HTTP_USERNAME: tms-event-user-2478
      WEBHOOK_URL: http://host.docker.internal:5000/api/client/user-registration-sync
      HTTP_EVENT_LISTENER_SECRET: tms-event-user-2478
    volumes:
      - ./sample-event-listener.jar:/opt/keycloak/providers/event-listener.jar
    command: start-dev --http-enabled=true
    restart: unless-stopped
```

More info on Event Listener on Keycloak SPI [https://dev.to/adwaitthattey/building-an-event-listener-spi-plugin-for-keycloak-2044]

## Weight & Dimensions Calculator

How To Calculate Dimensional Weight
What’s that? Maths is not one of your strengths?

Don’t worry, calculating a dimensional weight is just a matter of multiplying and dividing.

So worry not, let’s take a look at the formula to calculate a parcel’s dimensional weight!

(Parcel’s Length X Width X Height) / DIM Factor = Dimensional Weight (cm³/kg OR in³/lb)

Dimensional weight is calculated by multiplying the parcel’s length, width and height and dividing by the DIM factor that is determined by the shipping company.

The result will then be used to compare to the actual weight of the package. And as we’ve said just now, the higher of the two values will be used to calculate the shipping cost. Yes, it’s as simple as that.

What Is a DIM Factor?
A question you might have right now is what is a DIM Factor.

A Dimensional factor (DIM Factor), sometimes known as the DIM divisor is a set of numbers used to determine the DIM weight of a parcel. It represents the volume of a package allowed per unit of weight.

This set of numbers is determined by the courier company and each DIM factor is different depending on the type of shipping method and the courier company. DIM factor comes might differ depending if you're using the cubic inches per pound or cubic centimeters per kilogram.

Below is a more commonly seen DIM factor if your parcel is weighted in KG:

Sea Freight: (1:1000)
Air Freight: (1:5000)
Rail Freight (1:3000)
Road Freight: (1:3000)
\*Do take note that these DIM factors are just for reference and will vary based on company and if your parcel is weighted with pounds.

Freight Rates
Wondering what you'll need to do next once you’ve gotten the dimensional weight of your parcel? You'll next need to multiply it with the freight rate.

Freight rates are the price that a company imposes on helping clients or customers deliver their items. Each courier will have different freight rates that they impose on their customers.

So remember to check the freight rates for each mode of delivery with the courier company you desire to better clarify your shipping costs.

Example of Calculating Shipping Cost:
In this section let’s just create a scenario where you’d want to send out your parcel to your customers.

Say that you’d like to send a parcel using the courier’s air delivery. Take out your measuring tape and let’s measure the parcels together.

I’ll go first, here are the measurements of our parcel, the courier’s DIM Factor and Freight rates.

Dimensions (CCM): 120CM X 75CM x 60CM
The air freight DIM factor is 1:5000
Freight Rates: RM5/KG
As we’ve said just now, you will just need to incorporate the numbers that you’ve measured into the equation that we’ve provided just now. In our case, this makes it:

(120 X 75 X 60)/5000 = 108cm³/kg

Once we’ve found out the dimensional weight of the parcel, you will just need to use the amount and multiply it by the freight rate.

90 X RM5 = RM 450

And there you have it! Who would’ve thought that calculating the shipping fee for your parcels is that easy?

However, as we mentioned, if your physical weight is higher than the DIM weight, the courier will use the weight that has the greater value to calculate the shipping cost.
