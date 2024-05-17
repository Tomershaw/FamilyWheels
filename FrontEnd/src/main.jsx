import React from "react";
import ReactDOM from "react-dom/client";
import "../node_modules/@syncfusion/ej2-base/styles/material.css";
import "../node_modules/@syncfusion/ej2-buttons/styles/material.css";
import "../node_modules/@syncfusion/ej2-calendars/styles/material.css";
import "../node_modules/@syncfusion/ej2-dropdowns/styles/material.css";
import "../node_modules/@syncfusion/ej2-inputs/styles/material.css";
import "../node_modules/@syncfusion/ej2-lists/styles/material.css";
import "../node_modules/@syncfusion/ej2-navigations/styles/material.css";
import "../node_modules/@syncfusion/ej2-popups/styles/material.css";
import "../node_modules/@syncfusion/ej2-splitbuttons/styles/material.css";
import "../node_modules/@syncfusion/ej2-react-schedule/styles/material.css";
import "./file.css/editor.css";
import { registerLicense } from "@syncfusion/ej2-base";
import { RouterProvider } from "react-router-dom";
import { router } from "./router";
import "./index.css";


registerLicense(
  "Ngo9BigBOggjHTQxAR8/V1NBaF5cWWNCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXtfeXRdRWlfWUB+WUo="
);

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);
