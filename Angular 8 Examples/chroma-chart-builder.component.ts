import { Component, OnInit, ViewChild } from "@angular/core";
import { ChartConfigService } from "../services/chart-config.service";
import { ChartFilter, ChartConfig } from "../globals/chart-classes";
import { Router } from "@angular/router";

@Component({
  selector: "app-chroma-chart-builder",
  templateUrl: "./chroma-chart-builder.component.html",
  styleUrls: ["./chroma-chart-builder.component.css"]
})
export class ChromaChartBuilderComponent implements OnInit {
  chartConfig: ChartConfig = new ChartConfig();

  constructor(private service: ChartConfigService, private router: Router) {}

  // Style
  onStyleSelected(style: string) {
    this.chartConfig.type = style;
  }

  // Data for Line and Bar
  onAxisSelected(axis: { xAxisSelection: string; yAxisSelections: string[] }) {
    this.chartConfig.xAxis = axis.xAxisSelection;
    this.chartConfig.yAxis = axis.yAxisSelections;
    if (
      this.chartConfig.type == "VerticalBar" &&
      this.chartConfig.yAxis.length > 1
    )
      this.chartConfig.type = "GroupedVerticalBar";
  }

  // Data for Pie
  onDataSelected(data: { arrangeSelection: string; dataSelections: string[] }) {
    this.chartConfig.xAxis = data.arrangeSelection;
    this.chartConfig.yAxis = data.dataSelections;
  }

  filterSelected(filter: ChartFilter) {
    this.service.addConfig(this.chartConfig);
    this.router.navigate(["./dashboard"]);
  }

  ngOnInit() {}
}
