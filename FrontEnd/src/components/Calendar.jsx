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
import { extend } from "@syncfusion/ej2-base";
import { blockData } from "./datasource.json";
import { createElement } from "@syncfusion/ej2-base";
import { DropDownList } from "@syncfusion/ej2-react-dropdowns";
import { Ajax } from "@syncfusion/ej2-base";

//** Tomer Shaw was here */
/**
 * schedule block events sample
 */
const BlockEvents = () => {
  const [dataManager, setDataManager] = useState([]);
  const [dataReservations, setReservations] = useState([]);
  const [dataDriver, setDriver] = useState([]);

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
    console.log("dataManager2", dataManager);
    console.log("dataReservations", dataReservations);
    console.log("dataDriver", dataDriver);
  }, [dataManager, dataReservations, dataDriver]);

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
    if (args.element.classList.contains("e-appointment")) {
      const eventData = args.data;
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
      }
    }
  };

  function onPopupOpen(args) {
    if (args.type === "QuickInfo") {
      console.log(args);
      let formElement = args.element.querySelector(".e-subject");
      if (args.data.CarType === undefined) {
        formElement.classList.add("hidden");
      }
      formElement.innerText = args.data.CarType;
      args.data.CarType === undefined ? "" : args.data.CarType;
      console.log(formElement.innerText);
    }
    if (args.type === "Editor") {
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

        let dropDownList = new DropDownList({
          dataSource: [
            { text: "ibiza", value: "ibiza" },

            { text: "ionic", value: "ionic" },
          ],

          fields: { text: "text", value: "value" },

          value: args.data.CarType,

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
            <ScheduleComponent
              cssClass="block-events"
              width="100%"
              height="650px"
              // selectedDate={new Date(2024, 4, 2)}
              currentView="TimelineDay"
              resourceHeaderTemplate={resourceHeaderTemplate}
              eventSettings={{ dataSource: dataManager, format: fieldsData }}
              readonly={false}
              popupOpen={onPopupOpen}
              eventRendered={eventRendered}
              group={{ enableCompactView: true, resources: ["Driver"] }} // <===== I commented this out
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
