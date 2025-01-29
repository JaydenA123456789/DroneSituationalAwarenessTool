import '../styles/App.css';
import '../styles/Dashboard.css'
import DataRow from '../components/DataRow.jsx';
import CheckBoxRow from '../components/CheckBoxRow.jsx';

const TestUI = () => {
    return (
        <div>

            <div>
                <h1>New Page</h1>
                <p>Welcome to the new page!</p>
            </div>

            <div className="ui-container">
                <div className="control-panel">
                    <div className="info-section">
                        <div className="sub-container" id="csv-loader">CSV.csv</div>
                        <div className="sub-container" id="stats-section">
                            <DataRow dataTitle="Latitude" dataValue="test5" />
                            <DataRow dataTitle="Longitude" dataValue="test6" />
                            <DataRow dataTitle="Altitude" dataValue="test7" />
                            <DataRow dataTitle="Heading" dataValue="test8" />
                        </div>
                    </div>
                    <div className="play-pause-section">
                        <div id="circle-button-holder">
                            <button className="circle-button" id="small-button"></button>
                            <button className="circle-button" id="large-button"></button>
                            <button className="circle-button" id="small-button"></button>
                        </div>
                    </div>
                    <div className="info-section">
                        <div className="sub-container" id="toggle-section">
                            <CheckBoxRow checkboxTitle="Option" CheckedFunction={PrintToConsole1} UnCheckedFunction={PrintToConsole2} />
                            <CheckBoxRow checkboxTitle="Option" CheckedFunction={PrintToConsole1} UnCheckedFunction={PrintToConsole2} />
                            <CheckBoxRow checkboxTitle="Option" CheckedFunction={PrintToConsole1} UnCheckedFunction={PrintToConsole2} />
                            <CheckBoxRow checkboxTitle="Option" CheckedFunction={PrintToConsole1} UnCheckedFunction={PrintToConsole2} />

                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

function PrintToConsole1() {
    console.log("test1");
}

function PrintToConsole2() {
    console.log("test2");
}

export default TestUI;