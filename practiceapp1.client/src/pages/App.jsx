/* eslint-disable no-unused-vars */


import { useEffect, useState, useRef } from 'react';
import '../styles/App.css';
import Cesium3DWindow from '../components/Cesium3DWindow.jsx';
import DashboardOverlay from '../components/DashboardOverlay.jsx';
import * as Cesium from "cesium";
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';


function App() {
    const [stats, setStats] = useState({
        statsValue1: "",
        statsValue2: "",
        statsValue3: "",
        statsValue4: "",
    });

    const updateStats = (statsValue1, statsValue2, statsValue3, statsValue4) => {
        setStats({
            statsValue1,
            statsValue2,
            statsValue3,
            statsValue4,
        });
    };


    const cesiumRef = useRef();
    useEffect(() => {
        // 1. Build the connection
        const connection = new HubConnectionBuilder()
            .withUrl('https://localhost:7017/ClientHub')
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();


        // 2. Start the connection
        connection
            .start()
            .then(() => {
                console.log('SignalR connected.');

                // 3. Register handler for events
                connection.on('UpdateAddToCesium', (user, message) => {                 
                    const jsonEntity = JSON.parse(message);
                    var sphereColour;
                    var EntityType;
                    switch (user) {
                        case "SharedLibraries.EntityFunctionality.Debug_GenericEntity":
                            console.log('New Debug Data');
                            sphereColour = Cesium.Color.GREEN;
                            EntityType = "Debug";
                            AddSphere(0.2);
                            break;
                        case "SharedLibraries.EntityFunctionality.AirEntity":
                            console.log('New Air Data');
                            sphereColour = Cesium.Color.RED;
                            EntityType = "Aircraft";
                            AddSphere(0.2);
                            break;
                        case "SharedLibraries.EntityFunctionality.MaritimeEntity":
                            console.log('New Maritime Data');
                            sphereColour = Cesium.Color.YELLOW;
                            EntityType = "Ship";
                            AddSphere(0.2);
                            break;
                        case "SharedLibraries.EntityFunctionality.DroneEntity":
                            console.log('New Drone Data');
                            sphereColour = Cesium.Color.ORANGE;
                            EntityType = "Drone (MAVLink)";
                            //AddSphere(0.1)
                            AddModel();
                            break
                        case "Stale":
                            console.log('Data Staled');
                            EntityType = "Stale Vehicle";
                            sphereColour = Cesium.Color.GRAY;
                            AddSphere(0.25);
                            break;
                        default:
                            sphereColour = Cesium.Color.BLACK;
                            EntityType = "Unknown";
                            AddSphere(0.25);
                            break;
                    }

                    function AddSphere(colourAlpha) {
                        if (cesiumRef.current) {
                            cesiumRef.current.drawSphere(jsonEntity.Id,
                                jsonEntity.Position.Longitude, jsonEntity.Position.Latitude, jsonEntity.Position.Altitude,
                                200,
                                sphereColour, colourAlpha,
                                jsonEntity.Attitude.Roll, jsonEntity.Attitude.Pitch, jsonEntity.Attitude.Yaw,
                                EntityType, jsonEntity.LastUpdate_UTC
                            );
                        }
                    }
                    function AddModel() {
                        if (cesiumRef.current) {
                            cesiumRef.current.drawModel(jsonEntity.Id,
                                jsonEntity.Position.Longitude, jsonEntity.Position.Latitude, jsonEntity.Position.Altitude,
                                jsonEntity.Attitude.Roll, jsonEntity.Attitude.Pitch, jsonEntity.Attitude.Yaw-90
                            );
                        }
                    }
                });

                connection.on('UpdateTrackToCesium', (user, message) => {
                    const jsonEntity = JSON.parse(message);
                    var traceArray = jsonEntity.TracePositions;
                    if (cesiumRef.current) {
                        cesiumRef.current.AddUpdateTrack(jsonEntity.Id+"Trace", traceArray)
                    }
                });

                connection.on('DeleteFromCesium', (user, message) => {
                    if (cesiumRef.current) {
                        switch (user) {
                            case "EntityAndTrack":
                                cesiumRef.current.DeleteEntity(message);
                                cesiumRef.current.DeleteTrack(message);
                                break;
                            case "Entity":
                                cesiumRef.current.DeleteEntity(message);
                                break;
                            case "Track":
                                cesiumRef.current.DeleteTrack(message);
                                break;
                            default:
                                break;
                        }
                    }
                });
            })
            .catch((error) => console.error('Connection failed: ', error));

        // 4. Cleanup on unmount
        return () => {
            connection.stop().then(() => console.log('Connection stopped.'));
        };
    }, []);

    return (
        <div>
            {/* <div className="navbar"></div> */}
            <div className="main-screen">
                <Cesium3DWindow ref={cesiumRef} onUpdateStats={updateStats} />
                <DashboardOverlay stats={stats} onUpdateStats={updateStats} />

            </div>
        </div>
    );

}

export default App;