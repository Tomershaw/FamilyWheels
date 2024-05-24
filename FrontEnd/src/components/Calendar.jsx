import * as React from "react";
import { useEffect } from "react";
import { useState } from "react";
import {
  ScheduleComponent,
  ResourcesDirective,
  ResourceDirective,
  ViewsDirective,
  ViewDirective,
  Inject,
  TimelineViews,
  Resize,
  DragAndDrop,
  TimelineMonth,
  Day,
} from "@syncfusion/ej2-react-schedule";

import { createElement } from "@syncfusion/ej2-base";
import { DropDownList } from "@syncfusion/ej2-react-dropdowns";
import "react-toastify/dist/ReactToastify.css"; // Import the CSS styles
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

const toastConfig = {
  position: "top-right", // Position (top-left, top-center, etc.)
  autoClose: 5000, // Auto-close duration in milliseconds
  hideProgressBar: false, // Show or hide progress bar
  closeOnClick: true, // Close toast on click
  pauseOnHover: true, // Pause animation on hover
  draggable: true, // Allow dragging the toast
};

const BlockEvents = () => {
  const [dataManager, setDataManager] = useState([]);
  const [dataReservations, setReservations] = useState([]);
  const [dataDriver, setDriver] = useState([]);
  const [dataCar, setCar] = useState([]);
  const [currentUserId, setCurrentUserId] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      const token = localStorage.getItem("AccessToken");

      try {
        const response = await fetch("https://localhost:7189/reservations", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (response.ok) {
          const data = await response.json();
          const reservations = data.Reservations;
          setDataManager(reservations);
          console.log("parsedValue", data);
          const driverName = data.Drivers;
          setDriver(driverName);
          const carName = data.Cars;
          setCar(carName);
          setCurrentUserId(data.CurrentUserId);
        } else {
          console.error("API request failed:", response.status);
        }
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    console.log("dataManager", dataManager);
    console.log("dataDriver", dataDriver);
    console.log("Carname", dataCar);
    console.log("currentUserId", currentUserId);
  }, [dataManager, dataDriver, dataCar, currentUserId]);

  const handlePostReservation = async reservationData => {
    try {
      const token = localStorage.getItem("AccessToken");
      const response = await fetch("https://localhost:7189/Addreservations", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(reservationData),
      });

      if (response.ok) {
        const data = await response.json();
        console.log("Reservation saved:", data);
        // Perform any additional actions after successful save
      } else {
        console.error("API request failed:", response.status);
      }
    } catch (error) {
      console.error("Error saving reservation:", error);
    }
  };
  const handleDeleteReservation = async reservationId => {
    try {
      const token = localStorage.getItem("AccessToken");
      const response = await fetch("https://localhost:7189/Deletereservation", {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ id: reservationId }),
      });

      if (response.ok) {
        const data = await response.json();
        console.log("Reservation deleted:", data);
        toast.success("Reservation deleted successfully.");
      } else if (response.status === 403) {
        console.error("You are not authorized to delete this reservation.");
        toast.error("You are not authorized to delete this reservation.");
      } else {
        console.error("API request failed:", response.status);
        toast.error("An error occurred while deleting the reservation.");
      }
    } catch (error) {
      console.error("Error deleting reservation:", error);
      toast.error("An error occurred while deleting the reservation.");
    }
  };

  //   const data = extend([], blockData, null, true);

  //   const DriverName = ({ dataDriver }) => {
  //     const driverData = dataDriver.map(driver => ({
  //       Text: driver.DriverName,
  //       Id: driver.Id,
  //       GroupId: driver.GroupId,
  //       Color: driver.Color,
  //     }));
  //   };
  const eventRendered = args => {
    console.log(args.data);
    const eventData = args.data;
    console.log("DriverId", eventData.DriverId);
    console.log("currentUserId", currentUserId);
    if (eventData.DriverId && eventData.DriverId !== currentUserId) {
      // Apply a read-only style to the event
      args.element.classList.add("e-read-only");
    }

    if (args.element.classList.contains("e-appointment")) {
      const detailsElement = args.element.querySelector(
        ".e-appointment-details"
      );

      if (detailsElement) {
        const startTime = new Date(eventData.StartTime).toLocaleString(
          "en-US",
          { hour: "numeric", minute: "numeric", hour12: true }
        );
        const endTime = new Date(eventData.EndTime).toLocaleString("en-US", {
          hour: "numeric",
          minute: "numeric",
          hour12: true,
        });

        detailsElement.style.display = "flex";
        detailsElement.style.flexDirection = "column";
        detailsElement.style.alignItems = "flex-start";

        detailsElement.innerHTML = `
            <div class="e-subject" style="margin-left: 1px;">${eventData.CarType}</div>
            <div class="e-time">${startTime} - ${endTime}</div>
          `;

        // if (eventData.UserId !== currentUser.id) {
        //   args.element.classList.add("readonly-event");
        // }
      }
    }
  };
  const handleActionBegin = async args => {
    const eventData = args.data instanceof Array ? args.data[0] : args.data;

    console.log("Handling action begin:", args.requestType);

    if (
      args.requestType === "eventCreate" ||
      args.requestType === "eventChanged" ||
      args.requestType === "eventRemove"
    ) {
      const reservationData = {
        id: eventData.Id,
        isAllDay: eventData.IsAllDay,
        startTime: eventData.StartTime,
        endTime: eventData.EndTime,
        CarType: eventData.CarType,
        RecurrenceRule: eventData.RecurrenceRule,
        Description: eventData.Description,
      };

      console.log("Reservation data (create/change/delete):", reservationData);

      if (args.requestType === "eventRemove") {
        await handleDeleteReservation(eventData.Id);
        console.log("Deleting reservation with data:", eventData.Id);
      } else {
        await handlePostReservation(reservationData);
        console.log("Reservation data (create/change):", reservationData);
      }
    }
  };

  const handleActionFailure = async args => {
    const eventData = args.data instanceof Array ? args.data[0] : args.data;
    console.log("Handling action failure:", args.requestType);
  };
  // Function to display error popup with Lottie animation

  function onPopupOpen(args) {
    if (args.type === "QuickInfo") {
      console.log("args", args);
      let formElement = args.element.querySelector(".e-subject");

      if (args.data.CarType === undefined) {
        if (args.data && args.data.DriverId !== currentUserId) {
          args.cancel = true;
        } else {
          let formElement = args.element.querySelector(".e-schedule-form");
          if (formElement) {
            formElement.classList.add("hidden");
          }
        }
      } else if (args.data && args.data.DriverId !== currentUserId) {
        let deleteElement = args.element.querySelector(".e-delete");
        if (deleteElement) {
          deleteElement.style.display = "none"; // Set display to none
        }
        let editElement = args.element.querySelector(".e-edit");
        if (editElement) {
          editElement.style.display = "none"; // Set display to none
        }
      }
      formElement.innerText = args.data.CarType;
      args.data.CarType === undefined ? "" : args.data.CarType;
      // console.log(formElement.innerText);
    }

    if (args.type === "Editor") {
      if (args.data && args.data.DriverId !== currentUserId) {
        args.cancel = true;
      }
      if (!args.element.querySelector(".custom-field-row")) {
        let row = createElement("div", { className: "custom-field-row" });

        let formElement = args.element.querySelector(".e-schedule-form");

        formElement.firstChild.insertBefore(
          row,
          formElement.firstChild.firstChild
        );

        let container = createElement("div", {
          className: "custom-field-container",
        });

        let inputEle = createElement("input", {
          className: "e-field",
          attrs: { name: "Car Type" },
        });

        container.appendChild(inputEle);

        row.appendChild(container);

        const carDataSource = dataCar.map(car => ({
          text: car.CarName,
          value: car.CarName,
        }));

        console.log("Car Data Source for DropDownList:", carDataSource); // Log data source

        let dropDownList = new DropDownList({
          dataSource: carDataSource,
          fields: { text: "text", value: "value" },
          value: args.data.CarType || null,
          floatLabelType: "Always",
          placeholder: "Car type",
        });

        dropDownList.appendTo(inputEle);

        inputEle.setAttribute("name", "CarType");
      }
    }
  }

  const fieldsData = {
    id: "Id", // Assuming the unique identifier field is named "Id" in the API response
    subject: { name: "CarType" },
    location: { name: "Location" },
    description: { name: "Description" },
    startTime: { name: "StartTime" },
    endTime: { name: "EndTime" },
    recurrenceRule: { name: "RecurrenceRule" },
  };

  //   const eventSettings = { dataSource: dataManager, format: fieldsData };
  const getEmployeeName = value => {
    console.log("getEmployeeName", value);
    return value.resourceData[value.resource.textField];
  };
  // const getEmployeeImage = value => {
  //   return getEmployeeName(value).toLowerCase();
  // };
  const getEmployeeDesignation = value => {
    console.log("Designation", value);
    return value.resourceData.Designation;
  };
  const resourceHeaderTemplate = props => {
    return (
      <div className="template-wrap">
        <div className="employee-category">
          {/* <div className={"employee-image " + getEmployeeImage(props)} /> */}
          <div className="employee-name"> {getEmployeeName(props)}</div>
          <div className="employee-designation">
            {getEmployeeDesignation(props)}
          </div>
        </div>
      </div>
    );
  };
  return (
    <div className="schedule-control-section">
      <div className="col-lg-12 control-section">
        <div className="control-wrapper drag-sample-wrapper">
          <div className="schedule-container">
            <ToastContainer />
            <ScheduleComponent
              cssClass="block-events"
              width="100%"
              height="650px"
              // selectedDate={new Date(2024, 4, 2)}
              currentView="TimelineDay"
              resourceHeaderTemplate={resourceHeaderTemplate}
              //eventSettings={{ dataSource: dataManager, format: fieldsData }}
              eventSettings={{ dataSource: dataManager }}
              readonly={false}
              popupOpen={onPopupOpen}
              eventRendered={eventRendered}
              group={{ enableCompactView: true, resources: ["Driver"] }} // <===== I commented this out
              actionBegin={handleActionBegin}
              actionFailure={handleActionFailure}
            >
              <ResourcesDirective>
                <ResourceDirective
                  field="DriverId"
                  title="Driver"
                  name="Driver"
                  allowMultiple={true}
                  dataSource={dataDriver.map(driver => ({
                    Text: driver.DriverName,
                    Id: driver.Id,
                    GroupId: driver.GroupId,
                    Color: driver.Color,
                  }))}
                  textField="Text"
                  idField="Id"
                  colorField="Color"
                />
              </ResourcesDirective>
              <ViewsDirective>
                <ViewDirective option="Day" />
                <ViewDirective option="TimelineDay" />
                <ViewDirective option="TimelineMonth" />
                <ViewDirective
                  option="Week"
                  interval={7}
                  displayName=" Week"
                  showWeekend={true}
                  isSelected={false}
                />
                <ViewDirective
                  option="TimelineWeek"
                  showWeekend={true}
                  isSelected={true}
                />
              </ViewsDirective>
              <Inject
                services={[
                  Day,
                  TimelineViews,
                  TimelineMonth,
                  Resize,
                  DragAndDrop,
                ]}
              />
            </ScheduleComponent>
          </div>
        </div>
      </div>
    </div>
  );
};
export default BlockEvents;
