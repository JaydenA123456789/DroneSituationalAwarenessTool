/* eslint-disable react/prop-types */
import '../styles/App.css';
import '../styles/Dashboard.css'
import DataRow from './DataRow.jsx';
import CheckBoxRow from './CheckBoxRow.jsx';

const DashboardOverlay = ({ stats, onUpdateStats }) => {
    return (
        <div className="ui-container">
            <div className="control-panel">
                <div className="info-section">
                    
                    <div className="sub-container" id="stats-section">
                        <div id="title">Statistics</div>
                        <DataRow dataTitle="Latitude" dataValue={stats.statsValue1} />
                        <DataRow dataTitle="Longitude" dataValue={stats.statsValue2} />
                        <DataRow dataTitle="Altitude" dataValue={stats.statsValue3} />
                        <DataRow dataTitle="Heading" dataValue={stats.statsValue4}/>
                    </div>
                </div>
                <div className="play-pause-section">
                    <div id="circle-button-holder">
                    </div>
                </div>
                <div className="info-section">
                    <div className="sub-container" id="toggle-section">
                        <CheckBoxRow checkboxTitle="Jump To Vehicle" CheckedFunction={PrintToConsole1} UnCheckedFunction={PrintToConsole2} />
                        <CheckBoxRow checkboxTitle="Disable Tracks" CheckedFunction={PrintToConsole1} UnCheckedFunction={PrintToConsole2} />
                        <CheckBoxRow checkboxTitle="Option" CheckedFunction={PrintToConsole1} UnCheckedFunction={PrintToConsole2} />
                        <CheckBoxRow checkboxTitle="Option" CheckedFunction={PrintToConsole1} UnCheckedFunction={PrintToConsole2} />
                    </div>
                </div>
            </div>
        </div>
    );
    function PrintToConsole1() {
        console.log("test1");
        onUpdateStats("test1", "test2", "test3", "test4");
    }

    function PrintToConsole2() {
        console.log("test2");
    }
};





export default DashboardOverlay;