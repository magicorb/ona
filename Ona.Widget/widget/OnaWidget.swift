//
//  OnaWidget.swift
//  widget
//
//  Created by Natalia Naumova on 02/08/2024.
//

import WidgetKit
import SwiftUI

struct Provider: TimelineProvider {
    func placeholder(in context: Context) -> PeriodStateEntry {
        PeriodStateEntry(date: Date(), start: Date(), duration: 0, interval: 0)
    }

    func getSnapshot(in context: Context, completion: @escaping (PeriodStateEntry) -> ()) {
        let entry = PeriodStateEntry(date: Date(), start: Date(), duration: 0, interval: 0)
        completion(entry)
    }

    func getTimeline(in context: Context, completion: @escaping (Timeline<Entry>) -> ()) {
        let periodState = getPeriodState();
        
        let entry = PeriodStateEntry(date: Date(), start: periodState.start, duration: Int(periodState.duration), interval: Int(periodState.interval))

        let timeline = Timeline(entries: [entry], policy: .atEnd)
        
        completion(timeline)
    }
    
    func getPeriodState() -> PeriodState {
        let fallback = PeriodState(start: Date(), duration: 0, interval: 0)
        
        guard let userDefault = UserDefaults(suiteName: "group.com.natalianaumova.ona")?.object(forKey: "PeriodState") else {
            return fallback
        }
        let periodState = try? JSONDecoder().decode(PeriodState.self, from: userDefault as! Data)
        return periodState ?? fallback
    }
}

struct PeriodStateEntry: TimelineEntry {
    let date: Date
    let start: Date
    let duration: Int
    let interval: Int
}

struct OnaWidgetEntryView : View {
    var entry: Provider.Entry

    var body: some View {
        VStack {
            HStack {
                Text("Today:")
                Text(entry.date, style: .date)
            }
            HStack {
                Text("Start:")
                Text(entry.start, style: .date)
            }
            HStack {
                Text("Duration:")
                Text(String(entry.duration))
            }
            HStack {
                Text("Interval:")
                Text(String(entry.interval))
            }
        }
    }
}

struct OnaWidget: Widget {
    let kind: String = "PeriodTimes"

    var body: some WidgetConfiguration {
        StaticConfiguration(kind: kind, provider: Provider()) { entry in
            if #available(iOS 17.0, *) {
                OnaWidgetEntryView(entry: entry)
                    .containerBackground(.fill.tertiary, for: .widget)
            } else {
                OnaWidgetEntryView(entry: entry)
                    .padding()
                    .background()
            }
        }
        .configurationDisplayName("My Widget")
        .description("This is an example widget.")
    }
}

struct PeriodState : Codable {
    var start: Date
    var duration: Int
    var interval: Int
}

#Preview(as: .systemSmall) {
    OnaWidget()
} timeline: {
    PeriodStateEntry(date: .now, start: Date(), duration: 0, interval: 0)
}
