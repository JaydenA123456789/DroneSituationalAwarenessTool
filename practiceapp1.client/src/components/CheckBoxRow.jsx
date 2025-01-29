/* eslint-disable react/prop-types */
import '../component_styles/CheckBoxRow.css';

const CheckBoxRow = ({ checkboxTitle, CheckedFunction, UnCheckedFunction }) => {
    const handleChange = (event) => {
        if (event.target.checked) {
            CheckedFunction(); // Call the function when checked
        } else {
            UnCheckedFunction(); // Call the function when unchecked
        }
    };

    return (
        <div className="row">
            <div className="column">
                <label className="container">
                    <input type="checkbox" onChange={handleChange} />
                    <span className="checkmark"></span>
                </label>
            </div>
            <div className="column">
                {checkboxTitle}
            </div>
        </div>
    );
};

export default CheckBoxRow;
