using System.Collections.Generic;
using TinCan;

namespace DIG.GBLXAPI.Builders
{
	// ------------------------------------------------------------------------
	// "context": {
	// 		"extensions": {
	// 			"https://gblxapi.org/c3": [
	// 			{
	// 				"description": {
	// 				"en-US": "Social Studies C3 Standard d2.his.13.6-8"
	// 				},
	// 				"name": {
	// 				"en-US": "d2.his.13.6-8"
	// 				},
	// 				"id": "https://gblxapi.org/c3/d2-his-13-6-8"
	// 			}],
	// 			"https://w3id.org/xapi/gblxapi/extensions/domain": [
	// 			{
	// 				"name": {
	// 				"en-US": "History"
	// 				},
	// 				"id": "https://gblxapi.org/domain/history",
	// 				"description": {
	// 				"en-US": "Actor has been presented or interacted in Social Studies domain experiences in: History"
	// 				}
	// 			}]
	// 		},
	// 		"contextActivities": {
	// 			"category": [
	// 			{
	// 				"id": "https://gblxapi.org/socialstudies",
	// 				"objectType": "Activity"
	// 			}]
	// 			"grouping": [
	// 			{
	// 				"id": "https://dig-itgames.com/",
	// 				"objectType": "Activity"
	// 			},
	// 			{
	// 				"definition": {
	// 				"name": {
	// 					"en-US": "Three Digits - Easy"
	// 				},
	// 				"type": "https://w3id.org/xapi/gblxapi/activities/difficulty"
	// 				},
	// 				"id": "https://dig-itgames.com/three-digits/easy",
	// 				"objectType": "Activity"
	// 			}
	// 			],
	// 			"parent": [
	// 			{
	// 				"definition": {
	// 				"name": {
	// 					"en-US": "Three Digits"
	// 				},
	// 				"type": "https://w3id.org/xapi/seriousgames/activity-types/serious-game"
	// 				},
	// 				"id": "https://dig-itgames.com/three-digits",
	// 				"objectType": "Activity"
	// 			}]}
	// 		}
	//
	// parent: Activity directly related to current Statement Object
	// grouping: Any other related activities
	// category: Educational Subject Area of current Statement Object
	// extensions: Related educational standards
	// ------------------------------------------------------------------------
	public class ContextBuilder
    {
        public static ContextBuilder Start() { return new ContextBuilder(); }

        private ContextActivities _activities = new ContextActivities();
        private TinCan.Extensions _extensions;

        private ContextBuilder()
        {

        }

        // TODO: Overloads for handling single/multiple additions
        public ContextBuilder WithParents(List<Activity> parents)
        {
            _activities.parent = parents;

            return this;
        }

        public ContextBuilder WithGroupings(List<Activity> groupings)
        {
            _activities.grouping = groupings;

            return this;
        }

        public ContextBuilder WithCategories(List<Activity> categories)
        {
            _activities.category = categories;

            return this;
        }

        public ContextBuilder WithExtensions(TinCan.Extensions extensions)
        {
            _extensions = extensions;

            return this;
        }

        public Context Build()
        {
            Context context = new Context
            {
                contextActivities = _activities,
                extensions = _extensions
            };

            return context;
        }

		public static implicit operator Context(ContextBuilder builder) => builder.Build();
    }
}
