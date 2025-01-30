/* eslint-disable no-loss-of-precision */
/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
import '../component_styles/CustomCesium.css';
//import '../assets/fixedwing1.gltf';
import { useEffect, useImperativeHandle, forwardRef } from "react";
import * as Cesium from "cesium";

// Set your Cesium Ion access token

var viewerInstances = {};

var feedingEntityID;


// eslint-disable-next-line react/display-name
const Cesium3DWindow = forwardRef((props, ref) => {
    const { onUpdateStats: updateStats } = props;

    const drawModel = (_id, _Longitude, _Latitude, _Altitude, _Roll, _Pitch, _Yaw, _EntityType, _LastReport) => {
        try {
            //check if vehicle exists
            var entity = null;
            entity = viewerInstances[0].entities.getById(_id);


            const hpr = new Cesium.HeadingPitchRoll(
                Cesium.Math.toRadians(_Yaw),
                Cesium.Math.toRadians(_Pitch),
                Cesium.Math.toRadians(_Roll)
            );

            if (entity == null) {
                const newEntity = viewerInstances[0].entities.add({
                    id: _id,
                    customStats: [_EntityType, _Altitude.toFixed(5).toString(), _Yaw.toFixed(5).toString(), _LastReport],
                    customPosition: Cesium.Cartesian3.fromDegrees(_Longitude, _Latitude, _Altitude),
                    position: new Cesium.CallbackProperty(function () {
                        return newEntity.customPosition;
                    }, false),
                    customOrientation: Cesium.Transforms.headingPitchRollQuaternion(
                        Cesium.Cartesian3.fromDegrees(_Longitude, _Latitude, _Altitude), hpr),
                    orientation: new Cesium.CallbackProperty(function () {
                        return newEntity.customOrientation;
                    }, false),
                    model: {
                        uri: 'public/uav.glb',
                        minimumPixelSize: 528,
                        maximumScale: 0.08,
                        scale: 0.08,
                        silhouetteColor: Cesium.Color.BLUE,
                        silhouetteSize: parseFloat(1),
                    },
                });
            } else {
                var dynamicEntityPosition = new Cesium.CallbackProperty(function () {
                    return Cesium.Cartesian3.fromDegrees(
                        _Longitude,
                        _Latitude,
                        _Altitude
                    );
                }, false);
                entity.position = dynamicEntityPosition;

                const position = Cesium.Cartesian3.fromDegrees(_Longitude, _Latitude, _Altitude);
                const hpr = new Cesium.HeadingPitchRoll(Cesium.Math.toRadians(_Yaw), Cesium.Math.toRadians(_Pitch), Cesium.Math.toRadians(_Roll));
                const orientation = Cesium.Transforms.headingPitchRollQuaternion(position, hpr);
                entity.orientation = orientation;
                entity.customStats = [_EntityType, _Altitude.toFixed(2).toString(), _Yaw.toFixed(2).toString(), _LastReport];
            }
        } catch (error) {
            console.log('Sphere creation error: ', error);
        }
    }

    const drawSphere = (_id, _Longitude, _Latitude, _Altitude, _Radius, _Colour, _ColourAlpha, _Roll, _Pitch, _Yaw, _EntityType, _LastReport) => {
        try {
            //check if vehicle exists
            var entity = null;
            entity = viewerInstances[0].entities.getById(_id);

            if (entity == null) {
                const newEntity = viewerInstances[0].entities.add({
                    id: _id,
                    name: "Sphere with black outline",
                    customStats: [_EntityType, _Altitude.toFixed(5).toString(), _Yaw.toFixed(5).toString(), _LastReport],
                    customPosition: Cesium.Cartesian3.fromDegrees(_Longitude, _Latitude, _Altitude),
                    position: new Cesium.CallbackProperty(function () {
                        return newEntity.customPosition;
                    }, false),
                    ellipsoid: {
                        radii: new Cesium.Cartesian3(_Radius, _Radius, _Radius),
                        material: _Colour.withAlpha(_ColourAlpha),
                        outline: true,
                        outlineColor: Cesium.Color.BLACK.withAlpha(0.1),
                    },
                });
                newEntity.customPosition = Cesium.Cartesian3.fromDegrees(_Longitude, _Latitude, _Altitude);
            } else {
                var dynamicEntityPosition = new Cesium.CallbackProperty(function () {
                    return Cesium.Cartesian3.fromDegrees(
                        _Longitude,
                        _Latitude,
                        _Altitude
                    );
                }, false);
                entity.position = dynamicEntityPosition;
                entity.customStats = [_EntityType, _Altitude.toFixed(2).toString(), _Yaw.toFixed(2).toString(), _LastReport];
                entity.ellipsoid.material = _Colour.withAlpha(_ColourAlpha);
            }
        } catch (error) {
            console.log('JS error: ', error);
        }
    }

    const drawBillboard = (_id, _Longitude, _Latitude, _Altitude) => {
        try {
            //check if vehicle exists
            var entity = null;
            entity = viewerInstances[0].entities.getById(_id);

            if (entity == null) {
                const newEntity = viewerInstances[0].entities.add({
                    id: _id,
                    name: "Sphere with black outline",
                    customPosition: Cesium.Cartesian3.fromDegrees(_Longitude, _Latitude, _Altitude),
                    position: new Cesium.CallbackProperty(function () {
                        return newEntity.customPosition;
                    }, false),
                    billboard: {
                        image: "fixedWing_white.png",
                        show: true, 
                        pixelOffset: new Cesium.Cartesian2(0, 0), // default: (0, 0)
                        eyeOffset: new Cesium.Cartesian3(0.0, 0.0, 0.0), // default
                        horizontalOrigin: Cesium.HorizontalOrigin.CENTER, // default
                        verticalOrigin: Cesium.VerticalOrigin.BOTTOM, // default: CENTER
                        scale: 1.0, // default: 1.0
                        color: Cesium.Color.WHITE, // default: WHITE
                        rotation: Cesium.Math.PI_OVER_FOUR, // default: 0.0
                        alignedAxis: Cesium.Cartesian3.ZERO, // default
                        width: 40, // default: undefined
                        height: 40, // default: undefined
                        translucencyByDistance: new Cesium.NearFarScalar(1.85e4, 0.0, 1.5e5, 1.0),
                    },
                });
                newEntity.customPosition = Cesium.Cartesian3.fromDegrees(_Longitude, _Latitude, _Altitude);
            } else {
                var dynamicEntityPosition = new Cesium.CallbackProperty(function () {
                    return Cesium.Cartesian3.fromDegrees(
                        _Longitude,
                        _Latitude,
                        _Altitude
                    );
                }, false);
                entity.position = dynamicEntityPosition;
            }
        } catch (error) {
            console.log('JS error: ', error);
        }
    }

    const AddUpdateTrack = (_id, _traceArray) => {
        console.log(_id);
        try {
            if (_traceArray.length < 3) return;
            //check if vehicle exists
            var entity = null;
            entity = viewerInstances[0].entities.getById(_id);

            
            if (entity == undefined) {
                var positions = [];
                _traceArray.forEach(position => {
                    var test = typeof position.Longitude;
                    positions.push(Cesium.Cartesian3.fromDegrees(position.Longitude, position.Latitude, position.Altitude));
                });

                const newEntity = viewerInstances[0].entities.add({
                    id: _id,
                    customStats: ["", "", "", ""],
                    polyline: {
                        positions: new Cesium.CallbackProperty(function () {
                            return newEntity.customPositions;
                        }, false),
                        width: 2,
                        material: Cesium.Color.WHITE,
                    },
                });
                newEntity.customPositions = positions;

            } else {
                if (_traceArray.length > entity.customPositions.length) {
                entity.customPositions.push(
                    Cesium.Cartesian3.fromDegrees(
                        _traceArray[_traceArray.length - 1].Longitude,
                        _traceArray[_traceArray.length - 1].Latitude,
                        _traceArray[_traceArray.length - 1].Altitude
                    ));
                    const parentEntity = viewerInstances[0].entities.getById(_id.slice(0, -5));
                    entity.customStats = parentEntity._customStats;
                }
            }
            
        } catch (error) {
            console.log('Billboard error: ', error);
        }
    }

    const GetEntityStats = (entityID) => {
        const viewer = viewerInstances[0];
        const entity = viewer.entities.getById(entityID._id);
        return entity.customStats;
    }

    const DeleteEntity = (entityID) => {
        const viewer = viewerInstances[0];
        const entity = viewer.entities.getById(entityID);
        viewer.entities.remove(entity);
    }

    const DeleteTrack = (entityID) => {
        const viewer = viewerInstances[0];
        const entity = viewer.entities.getById(entityID+"Trace");
        viewer.entities.remove(entity);
    }

    useImperativeHandle(ref, () => ({
        drawModel, drawSphere, drawBillboard, AddUpdateTrack, GetEntityStats, DeleteEntity, DeleteTrack,
    }));

    useEffect(() => {
        // Initialize Cesium after the component mounts
        InitialiseCesium("cesiumContainer");
        InitialiseStatsFeed();
    }, []);

    return (
        <div
            id="cesiumContainer"
            style={{
                width: "100%",
                height: "100vh",
                position: "relative",
            }}
        />
    );

    //globalTrackedEntity:
    
    function InitialiseStatsFeed() {
        if (!feedingEntityID) {
            updateStats("", "", "", "");
            setTimeout(InitialiseStatsFeed, 1000);
            return;
        }

        const feedingEntity = viewerInstances[0].entities.getById(feedingEntityID);
        if (feedingEntity !== undefined) {
            const entityStats = feedingEntity.customStats;
            if (Array.isArray(entityStats) && entityStats.length >= 4) {
                updateStats(entityStats[0], entityStats[1], entityStats[2], entityStats[3]);
            } else {
                console.log("GetEntityStats did not return a valid array with at least 4 elements.");
            }

        }
        setTimeout(InitialiseStatsFeed, 1000);
    }

    //At this point this function should be broken down to sub functions for maintainability
    function InitialiseCesium(containerId) {
        // Prevent reinitialization by checking if an instance already exists
        if (viewerInstances[containerId]) {
            return viewerInstances[containerId]; // Return the existing viewer
        }

        //should be in .env file
        Cesium.Ion.defaultAccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0M2RhNmNiMS02YmZhLTQ2NmUtYjJjMi01YWIyYzEzOWM1YzIiLCJpZCI6MTk5NjI5LCJpYXQiOjE3Mzc3MDMyNzh9.dyxQs1SeNFogZ78wz0cxAY22NTeQZVWxPuk6xq9EeYk";

        feedingEntityID = null;

        var viewer = new Cesium.Viewer(containerId, {
            homeButton: false,
            navigationHelpButton: false,
            selectionIndicator: false,
            infoBox: false,
            fullscreenButton: false,
            terrain: Cesium.Terrain.fromWorldTerrain(),
            animation: false,
            timeline: false,
            geocoder: false,
            baseLayerPicker: false,
            creditContainer: document.createElement('div') //empty div where branding was
        });

        const scene = viewer.scene;

        //performance adjustments
        viewer.scene.requestRenderMode = true;
        viewer.scene.maximumRenderTimeChange = 0.1;
        viewer.scene.preRender
        viewer.scene.fxaa = true;
        scene.performanceDisplay

        viewer.camera.setView({
            destination: Cesium.Cartesian3.fromDegrees(
                153.12345678,
                -27.1234567,
                23000.0
            ),
        });

        viewer.entities.add({
            id: 'mou',
            label: {
                show: true
            }
        });
        viewer.scene.canvas.addEventListener('mousemove', function (e) {
            var entity = viewer.entities.getById('mou');
            if (entity === undefined) {
                viewer.entities.add({
                    id: 'mou',
                    label: {
                        show: true
                    }
                });
                entity = viewer.entities.getById('mou');
            }

            var ellipsoid = viewer.scene.globe.ellipsoid;
            // Mouse over the globe to see the cartographic position 
            var cartesian = viewer.camera.pickEllipsoid(new Cesium.Cartesian3(e.clientX, e.clientY), ellipsoid);
            if (cartesian) {
                var cartographic = ellipsoid.cartesianToCartographic(cartesian);
                var longitudeString = Cesium.Math.toDegrees(cartographic.longitude).toFixed(10);
                var latitudeString = Cesium.Math.toDegrees(cartographic.latitude).toFixed(10);
                if (typeof viewer.scene.globe.getHeight(cartographic) !== 'undefined') {
                    var altitudeString = viewer.scene.globe.getHeight(cartographic).toFixed(2);
                }

                entity.position = cartesian;
                entity.label.show = false;
                entity.label.font_style = 84;
                //entity.position= Cesium.Cartesian2.ZERO; 
                entity.label.text = 'Lat:  ' + latitudeString + ', Long: ' + longitudeString + ', Alt:  ' + altitudeString;
                var result = entity.label.text;  // We can reuse this
                let element = document.getElementById("mouseCoordinates");
                if (element && element !== undefined) {
                    element.innerHTML = result;
                }
            } else {
                entity.label.show = false;
            }
        });

        const clickHandler = new Cesium.ScreenSpaceEventHandler(viewer.scene.canvas);
        const onClick = function (movement) {
            try {
                const hit = viewer.scene.pick(movement.position);
                if (Cesium.defined(hit) && Cesium.defined(hit.id)) {
                    const entityStats = GetEntityStats(hit.id);

                    if (Array.isArray(entityStats) && entityStats.length >= 4) {
                        updateStats(entityStats[0], entityStats[1], entityStats[2], entityStats[3]);
                        feedingEntityID = hit.id._id;
                    } else {
                        console.log("GetEntityStats did not return a valid array with at least 4 elements.");
                    }
                }
            } catch (ex) {
                console.log("Exception: ", ex);
            }
        };
        clickHandler.setInputAction(onClick, Cesium.ScreenSpaceEventType.LEFT_CLICK);

        const labelEntity = viewer.entities.add({
            label: {
                show: false,
                showBackground: true,
                font: "14px monospace",
                horizontalOrigin: Cesium.HorizontalOrigin.LEFT,
                verticalOrigin: Cesium.VerticalOrigin.TOP,
                pixelOffset: new Cesium.Cartesian2(15, 0),
            },
        });

        // Mouse over the globe to see the cartographic position
        const hoverHandler = new Cesium.ScreenSpaceEventHandler(scene.canvas);
        hoverHandler.setInputAction(function (movement) {
            let foundPosition = false;
            let longitudeString = 0;
            let latitudeString = 0;
            let heightString = 0;

            const scene = viewer.scene;
            if (scene.mode !== Cesium.SceneMode.MORPHING) {
                if (scene.pickPositionSupported) {
                    const cartesian = viewer.scene.pickPosition(movement.endPosition);

                    if (Cesium.defined(cartesian)) {
                        const cartographic = Cesium.Cartographic.fromCartesian(cartesian);
                        longitudeString = Cesium.Math.toDegrees(
                            cartographic.longitude,
                        ).toFixed(2);
                        latitudeString = Cesium.Math.toDegrees(
                            cartographic.latitude,
                        ).toFixed(2);
                        heightString = cartographic.height.toFixed(2);

                        labelEntity.position = cartesian;
                        labelEntity.label.show = true;
                        labelEntity.label.text =
                            `\nLon: ${`   ${longitudeString}`.slice(-7)}\u00B0` +
                            `\nLat: ${`   ${latitudeString}`.slice(-7)}\u00B0` +
                            `\nAlt: ${`   ${heightString}`.slice(-7)}m`;

                        labelEntity.label.eyeOffset = new Cesium.Cartesian3(
                            0.0,
                            0.0,
                            -cartographic.height *
                            (scene.mode === Cesium.SceneMode.SCENE2D ? 1.5 : 1.0),
                        );

                        foundPosition = true;
                    }
                }
            }

            if (!foundPosition) {
                labelEntity.label.show = false;
            } else {
                const pickedObject = scene.pick(movement.endPosition);
                if (pickedObject != undefined) {
                    //console.log(pickedObject._id);
                    labelEntity.label.text = "Vehicle ID: " + pickedObject.id._id +
                        `\nLon: ${`   ${longitudeString}`.slice(-7)}\u00B0` +
                        `\nLat: ${`   ${latitudeString}`.slice(-7)}\u00B0`;
                    labelEntity.label.show = true;
                } else {
                    labelEntity.label.show = false;
                }
            }
        }, Cesium.ScreenSpaceEventType.MOUSE_MOVE);
        viewerInstances[0] = viewer;
    };
});

export default Cesium3DWindow;
