/* eslint-disable react/prop-types */
import '../component_styles/DataRow.css';

const DataRow = ({ dataTitle, dataValue }) => {
    return (
        <div className="row">
            <div className="column">
                {dataTitle}
            </div>
            <div className="column">
                {dataValue}
            </div>
        </div>
    );
};

export default DataRow;