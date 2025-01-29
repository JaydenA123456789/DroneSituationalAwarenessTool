# DroneSituationalAwarenessTool
Requirements:
    -Visual Studio with .Net 8
    -Any modern browser
    -Mission Planner (Windows)

Steps to running program:
    1. Open DroneSituationalAwarenessTool.sln with Visual Studio
    2. Next to "Start" click the down arrow
    3. Select "Configure Startup Projects"
    4. Under 'Action' choose "Start" for:
        (Mandatory):
        DroneSituationalAwarenessTool.Client
        DroneSituationalAwarenessTool.Server
        (Optional)
        MavLinkMicroService
        AirDataMicroService
        MaritimeDataMicroService
    5. In Mission Planner go to "Simulation" and select the vehicle
    6. Add waypoints or a planned waypoint system
    7. Begin the flight (vehicle dependent. Usually take off, arm then set to auto)
    8. Press CTRL+F to enter developer modern
    9. Press "MAVLink"
    10. Open a TCP Host with Write checked (This step might not be necessary)
    11. In the top row, add "TCP"(Type), "Outbound"(Direction), 4112(Port), 127.0.0.1(Host)
    12. Press go. If the program is open you might need to refresh the browser to reopen the connection
        -This step can be very finicky since the tool isnt officially released