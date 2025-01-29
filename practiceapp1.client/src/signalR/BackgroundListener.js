import { useEffect, useRef } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

import * as Cesium from "cesium";

const BackgroundListener = () => {
    const cesiumRef = useRef();

    useEffect(() => {
        // 1. Build the connection
        const connection = new HubConnectionBuilder()
            .withUrl('https://localhost:7017/ClientHub') // Replace with your actual hub URL
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        // 2. Start the connection
        connection
            .start()
            .then(() => {
                console.log('SignalR connected.');

                // 3. Register handler for the "ReceiveMessage" event
                connection.on('UpdateAddToCesium', (user, message) => {
                    console.log(`Received message from ${user}: ${message}`);

                    const jsonEntity = JSON.parse(message);

                    if (cesiumRef.current) {
                        cesiumRef.current.drawBox(jsonEntity.Position.Latitude, jsonEntity.Position.Longitude, jsonEntity.Position.Altitude,
                            50, 50, 50,
                            Cesium.Color.RED, 0.5
                        );
                    }

                });
            })
            .catch((error) => console.error('Connection failed: ', error));

        // 4. Cleanup on unmount
        return () => {
            connection.stop().then(() => console.log('Connection stopped.'));
        };
    }, []);

    // No UI to render, so return null
    return null;
};

export default BackgroundListener;
