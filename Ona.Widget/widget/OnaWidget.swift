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
        PeriodStateEntry(date: Date(), emoji: "😀", count: 0)
    }

    func getSnapshot(in context: Context, completion: @escaping (PeriodStateEntry) -> ()) {
        let entry = PeriodStateEntry(date: Date(), emoji: "😀", count: 0)
        completion(entry)
    }

    func getTimeline(in context: Context, completion: @escaping (Timeline<Entry>) -> ()) {
        var entries: [PeriodStateEntry] = []

        let count = getCount()
                
        // Generate a timeline consisting of five entries an hour apart, starting from the current date.
        let currentDate = Date()
        for hourOffset in 0 ..< 5 {
            let entryDate = Calendar.current.date(byAdding: .hour, value: hourOffset, to: currentDate)!
            let entry = PeriodStateEntry(date: entryDate, emoji: "😀", count: count)
            entries.append(entry)
        }

        let timeline = Timeline(entries: entries, policy: .atEnd)
        completion(timeline)
    }
    
    func getCount() -> Int {
        let fallback = 0;
        
        guard let userDefaults = UserDefaults(suiteName: "group.com.natalianaumova.ona") else {
            return fallback
        }
        return userDefaults.integer(forKey: "Count")// ?? fallback
        //return 4
    }
}

struct PeriodStateEntry: TimelineEntry {
    let date: Date
    let emoji: String
    let count: Int
}

struct OnaWidgetEntryView : View {
    var entry: Provider.Entry

    var body: some View {
        VStack {
            Text("Now:")
            Text(entry.date, style: .time)

            Text("Emoji:")
            Text(entry.emoji)
            
            Text("Count:")
            Text("\(entry.count)")
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

#Preview(as: .systemSmall) {
    OnaWidget()
} timeline: {
    PeriodStateEntry(date: .now, emoji: "😀", count: 0)
    PeriodStateEntry(date: .now, emoji: "🤩", count: 0)
}
