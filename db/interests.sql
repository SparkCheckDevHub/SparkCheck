-- --------------------------------------------------------------------------------
-- Name: James Glenn
-- Project: SparkCheck
-- Abstract: SparkCheck Stored Interests
-- --------------------------------------------------------------------------------

-- --------------------------------------------------------------------------------
-- Options
-- --------------------------------------------------------------------------------

-- Wait for DB to be created
WHILE DB_ID('dbSparkCheck') IS NULL
BEGIN
    PRINT 'Waiting for database dbSparkCheck...'
    WAITFOR DELAY '00:00:05'
END

-- Switch to DB
USE dbSparkCheck;
GO

SET NOCOUNT ON;
GO

-- Wait for relevant tables to be created
WHILE OBJECT_ID('dbo.TInterests', 'U') IS NULL
   OR OBJECT_ID('dbo.TInterestCategories', 'U') IS NULL
   OR OBJECT_ID('dbo.TInterestSubCategories', 'U') IS NULL
BEGIN
    PRINT 'Waiting for tables...'
	WAITFOR DELAY '00:00:05'
END

-- Exit if tables are already populated
IF EXISTS (SELECT 1 FROM dbo.TInterests)
    THROW 50000, 'TInterests table already populated', 1;

IF EXISTS (SELECT 1 FROM dbo.TInterestCategories)
    THROW 50001, 'TInterestCategories table already populated', 1;

IF EXISTS (SELECT 1 FROM dbo.TInterestSubCategories)
    THROW 50002, 'TInterestSubCategories table already populated', 1;

-- --------------------------------------------------------------------------------
-- Insert Values
-- --------------------------------------------------------------------------------

-- Top-Level Interest Categories
INSERT INTO TInterestCategories ( strInterestCategory )
VALUES ( 'Food' ) -- 1
      ,( 'Music' ) -- 2
      ,( 'Movies & TV' ) -- 3
      ,( 'Lifestyle' ) -- 4
      ,( 'Hobbies & Activities' ) -- 5
      ,( 'Relationship Goals' ) -- 6
      ,( 'Personality Types' ) -- 7
      ,( 'Political Beliefs' ) -- 8

-- Distinct Interest Sub Categories
INSERT INTO TInterestSubCategories ( strInterestSubCategory )
VALUES ( 'Dietary Lifestyle' ) -- 1 Start of Food
      ,( 'Eating Habits & Interests' ) -- 2
      ,( 'Food Allergies / Sensitivities' ) -- 3
      ,( 'Favorite Cuisine' ) -- 4
      ,( 'Favorite Music Genre' ) -- 5 Start of Music
      ,( 'Listening Habits' ) -- 6
      ,( 'Favorite Decade' ) -- 7
      ,( 'Favorite Movie/TV Genre' ) -- 8 Start of Movies & TV
      ,( 'Viewing Habits' ) -- 9
      ,( 'Favorite Platform' ) -- 10
      ,( 'Daily Habits' ) -- 11 Start of Lifestyle Preferences
      ,( 'Health & Fitness' ) -- 12
      ,( 'Diet & Nutrition' ) -- 13
      ,( 'Substance Use' ) -- 14
      ,( 'Living Situation' ) -- 15
      ,( 'Transportation' ) -- 16
      ,( 'Tech & Social Media' ) -- 17
      ,( 'Creative & Artist' ) -- 18 Start of Hobbies & Activities
      ,( 'Active & Outdoorsy' ) -- 19
      ,( 'Fitness & Wellness' ) -- 20
      ,( 'Gaming & Tech' ) -- 21
      ,( 'Entertainment & Pop Culture' ) -- 22
      ,( 'Culinary & Foodie' ) -- 23
      ,( 'Social & Community' ) -- 24
      ,( 'Learning & Personal Growth' ) -- 25
      ,( 'Intentions' ) -- 26 Start of Relationship Goals
      ,( 'Relationship Style' ) -- 27
      ,( 'Family Plans' ) -- 28
      ,( 'Living Preferences' ) -- 29
      ,( 'Romantic Values' ) -- 30
      ,( 'Social Style' ) -- 31 Start of Personality Types
      ,( 'Emotional Style' ) -- 32
      ,( 'Decision-Making Style' ) -- 33
      ,( 'Energy & Vibe' ) -- 34
      ,( 'General Alignment' ) -- 35 Start of Political Beliefs
      ,( 'Engagement Level' ) -- 36
      ,( 'Key Preferences' ) -- 37

-- Food -> Dietary Lifestyle
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Vegetarian', 1, 1 )
      ,( 'Vegan', 1, 1 )
      ,( 'Pescatarian', 1, 1 )
      ,( 'Omnivore', 1, 1 )
      ,( 'Carnivore (Meat Lover)', 1, 1 )
      ,( 'Plant-Based', 1, 1 )
      ,( 'Gluten-Free', 1, 1 )
      ,( 'Dairy-Free', 1, 1 )
      ,( 'Keto', 1, 1 )
      ,( 'Paleo', 1, 1 )
      ,( 'Halal', 1, 1 )
      ,( 'Kosher', 1, 1 )
      ,( 'Organic Only', 1, 1 )

-- Food -> Eating Habits & Interests
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Love Trying New Foods', 1, 2 )
      ,( 'Not Adventurous with Food', 1, 2 )
      ,( 'Sweet Tooth', 1, 2 )
      ,( 'Spicy Food Lover', 1, 2 )
      ,( 'Comfort Food Fan', 1, 2 )
      ,( 'Seafood Lover', 1, 2 )
      ,( 'Sushi Addict', 1, 2 )
      ,( 'Pizza Obsessed', 1, 2 )
      ,( 'Coffee Enthusiast', 1, 2 )
      ,( 'Tea Over Coffee', 1, 2 )
      ,( 'Cook at Home', 1, 2 )
      ,( 'Prefer Dining Out', 1, 2 )
      ,( 'No Fast Food', 1, 2 )

-- Food -> Food Allergies / Sensitivities
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Nut Allergy', 1, 3 )
      ,( 'Dairy Allergy', 1, 3 )
      ,( 'Gluten Sensitivity', 1, 3 )
      ,( 'Shellfish Allergy', 1, 3 )
      ,( 'Soy Allergy', 1, 3 )
      ,( 'Egg Allergy', 1, 3 )

-- Food -> Favorite Cuisine
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Italian', 1, 4 )
      ,( 'Mexican', 1, 4 )
      ,( 'Chinese', 1, 4 )
      ,( 'Thai', 1, 4 )
      ,( 'Indian', 1, 4 )
      ,( 'Japanese', 1, 4 )
      ,( 'Mediterranean', 1, 4 )
      ,( 'American', 1, 4 )
      ,( 'Middle Eastern', 1, 4 )
      ,( 'Korean', 1, 4 )
      ,( 'French', 1, 4 )
      ,( 'Vietnamese', 1, 4 )

-- Music -> Favorite Music Genre
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Pop', 2, 5 )
      ,( 'Rock', 2, 5 )
      ,( 'Hip-Hop / Rap', 2, 5 )
      ,( 'R&B / Soul', 2, 5 )
      ,( 'Country', 2, 5 )
      ,( 'Jazz', 2, 5 )
      ,( 'Blues', 2, 5 )
      ,( 'EDM / Electronic', 2, 5 )
      ,( 'Classical', 2, 5 )
      ,( 'Reggae', 2, 5 )
      ,( 'Metal', 2, 5 )
      ,( 'Punk', 2, 5 )
      ,( 'Indie / Alternative', 2, 5 )
      ,( 'Latin', 2, 5 )
      ,( 'Gospel', 2, 5 )
      ,( 'K-Pop', 2, 5 )
      ,( 'Lo-Fi', 2, 5 )
      ,( 'Folk', 2, 5 )
      ,( 'Soundtracks / Scores', 2, 5 )

-- Music -> Listening Habits
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Music All Day', 2, 6 )
      ,( 'Just Background Noise', 2, 6 )
      ,( 'Only When Driving', 2, 6 )
      ,( 'Headphones Always', 2, 6 )
      ,( 'Prefer Live Music', 2, 6 )
      ,( 'Enjoy Singing Along', 2, 6 )
      ,( 'Play an Instrument', 2, 6 )
      ,( 'Create My Own Music', 2, 6 )
      ,( 'Go to Concerts Often', 2, 6 )
      ,( 'Love Music Festivals', 2, 6 )
      ,( 'Like Discovering New Artists', 2, 6 )
      ,( 'Stick to the Classics', 2, 6 )
      ,( 'Make Playlists for Everything', 2, 6 )

-- Music -> Favorite Decade
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( '60s & Earlier', 2, 7 )
      ,( '70s', 2, 7 )
      ,( '80s', 2, 7 )
      ,( '90s', 2, 7 )
      ,( '2000s', 2, 7 )
      ,( '2010s', 2, 7 )
      ,( 'Today’s Hits', 2, 7 )

-- Movies & TV -> Favorite Movie/TV Genre
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Action', 3, 8 )
      ,( 'Comedy', 3, 8 )
      ,( 'Drama', 3, 8 )
      ,( 'Thriller', 3, 8 )
      ,( 'Horror', 3, 8 )
      ,( 'Romance', 3, 8 )
      ,( 'Sci-Fi', 3, 8 )
      ,( 'Fantasy', 3, 8 )
      ,( 'Mystery', 3, 8 )
      ,( 'Documentary', 3, 8 )
      ,( 'Crime', 3, 8 )
      ,( 'Adventure', 3, 8 )
      ,( 'Animation', 3, 8 )
      ,( 'Superhero / Comic Book', 3, 8 )
      ,( 'Historical / Period Pieces', 3, 8 )
      ,( 'Reality TV', 3, 8 )
      ,( 'Anime', 3, 8 )
      ,( 'Sitcoms', 3, 8 )
      ,( 'Game Shows', 3, 8 )
      ,( 'True Crime', 3, 8 )

-- Movies & TV -> Viewing Habits
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Movie Buff', 3, 9 )
      ,( 'Binge Watcher', 3, 9 )
      ,( 'Casual Viewer', 3, 9 )
      ,( 'Prefer Series Over Movies', 3, 9 )
      ,( 'Prefer Movies Over Series', 3, 9 )
      ,( 'Go to Theaters Often', 3, 9 )
      ,( 'Stream Everything', 3, 9 )
      ,( 'Rewatch Favorites', 3, 9 )
      ,( 'Only Watch New Releases', 3, 9 )
      ,( 'Enjoy Foreign Films', 3, 9 )
      ,( 'Prefer Subtitles', 3, 9 )
      ,( 'Watch With Partner', 3, 9 )
      ,( 'Watch Alone', 3, 9 )
      ,( 'Nighttime Viewer', 3, 9 )
      ,( 'Background Noise Only', 3, 9 )

-- Movies & TV -> Favorite Platform
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Netflix', 3, 10 )
      ,( 'Hulu', 3, 10 )
      ,( 'Disney+', 3, 10 )
      ,( 'HBO Max', 3, 10 )
      ,( 'Amazon Prime Video', 3, 10 )
      ,( 'Apple TV+', 3, 10 )
      ,( 'YouTube', 3, 10 )
      ,( 'Crunchyroll', 3, 10 )
      ,( 'Peacock', 3, 10 )
      ,( 'Paramount+', 3, 10 )

-- Lifestyle -> Daily Habits
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Early Bird', 4, 11 )
      ,( 'Night Owl', 4, 11 )
      ,( 'Light Sleeper', 4, 11 )
      ,( 'Heavy Sleeper', 4, 11 )
      ,( 'Neat Freak', 4, 11 )
      ,( 'Laid-Back', 4, 11 )
      ,( 'Always On the Go', 4, 11 )
      ,( 'Homebody', 4, 11 )
      ,( 'Workaholic', 4, 11 )
      ,( 'Balanced Routine', 4, 11 )

-- Lifestyle -> Health & Fitness
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Regular Exerciser', 4, 12 )
      ,( 'Occasional Workouts', 4, 12 )
      ,( 'Prefer Walking', 4, 12 )
      ,( 'Yoga / Meditation Enthusiast', 4, 12 )
      ,( 'Fitness is a Priority', 4, 12 )
      ,( 'Not Into Fitness', 4, 12 )
      ,( 'Follow a Wellness Routine', 4, 12 )
      ,( 'Open to New Health Trends', 4, 12 )

-- Lifestyle -> Diet & Nutrition
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Eat Clean', 4, 13 )
      ,( 'Eat What I Want', 4, 13 )
      ,( 'Meal Prep Regularly', 4, 13 )
      ,( 'Frequent Snacker', 4, 13 )
      ,( 'Intermittent Fasting', 4, 13 )
      ,( 'Big on Supplements', 4, 13 )
      ,( 'Rarely Cook', 4, 13 )
      ,( 'Prefer Home-Cooked Meals', 4, 13 )

-- Lifestyle -> Substance Use
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Doesn’t Drink', 4, 14 )
      ,( 'Social Drinker', 4, 14 )
      ,( 'Regular Drinker', 4, 14 )
      ,( 'Doesn’t Smoke', 4, 14 )
      ,( 'Smokes Occasionally', 4, 14 )
      ,( 'Smokes Regularly', 4, 14 )
      ,( '420 Friendly', 4, 14 )
      ,( 'Sober Lifestyle', 4, 14 )

-- Lifestyle -> Living Situation
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Lives Alone', 4, 15 )
      ,( 'Lives With Roommates', 4, 15 )
      ,( 'Lives With Family', 4, 15 )
      ,( 'Has Children', 4, 15 )
      ,( 'No Children', 4, 15 )
      ,( 'Open to Living Together Someday', 4, 15 )
      ,( 'Prefers Own Space', 4, 15 )

-- Lifestyle -> Transportation
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Drives Own Car', 4, 16 )
      ,( 'Uses Public Transit', 4, 16 )
      ,( 'Rideshare User', 4, 16 )
      ,( 'Bikes Everywhere', 4, 16 )
      ,( 'Walks Most Places', 4, 16 )

-- Lifestyle -> Tech & Social Media
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Always Online', 4, 17 )
      ,( 'Minimal Tech Use', 4, 17 )
      ,( 'Active on Social Media', 4, 17 )
      ,( 'Rarely Posts', 4, 17 )
      ,( 'Detoxes from Tech Often', 4, 17 )
      ,( 'Enjoys Digital Minimalism', 4, 17 )

-- Hobbies & Activities -> Creative & Artist
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Drawing / Painting', 5, 18 )
      ,( 'Photography', 5, 18 )
      ,( 'Writing / Journaling', 5, 18 )
      ,( 'Playing Music', 5, 18 )
      ,( 'Singing', 5, 18 )
      ,( 'DIY / Crafting', 5, 18 )
      ,( 'Acting / Theater', 5, 18 )
      ,( 'Dancing', 5, 18 )
      ,( 'Filmmaking', 5, 18 )
      ,( 'Designing (Graphic, Fashion, etc.)', 5, 18 )

-- Hobbies & Activities -> Active & Outdoorsy
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Hiking', 5, 19 )
      ,( 'Biking', 5, 19 )
      ,( 'Running / Jogging', 5, 19 )
      ,( 'Swimming', 5, 19 )
      ,( 'Camping', 5, 19 )
      ,( 'Climbing', 5, 19 )
      ,( 'Kayaking / Canoeing', 5, 19 )
      ,( 'Beach Days', 5, 19 )
      ,( 'Road Trips', 5, 19 )
      ,( 'Traveling Abroad', 5, 19 )
      ,( 'Adventure Sports (e.g., skydiving, ziplining)', 5, 19 )

-- Hobbies & Activities -> Fitness & Wellness
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Gym Workouts', 5, 20 )
      ,( 'Yoga', 5, 20 )
      ,( 'Pilates', 5, 20 )
      ,( 'Martial Arts', 5, 20 )
      ,( 'CrossFit', 5, 20 )
      ,( 'Meditation', 5, 20 )
      ,( 'Group Classes', 5, 20 )
      ,( 'Sports Leagues', 5, 20 )

-- Hobbies & Activities -> Gaming & Tech
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Video Games', 5, 21 )
      ,( 'Board Games', 5, 21 )
      ,( 'Tabletop RPGs (D&D, etc.)', 5, 21 )
      ,( 'Puzzle Games', 5, 21 )
      ,( 'Coding / Programming', 5, 21 )
      ,( 'Building PCs', 5, 21 )
      ,( 'Crypto / NFTs', 5, 21 )
      ,( 'Watching Esports', 5, 21 )

-- Hobbies & Activities -> Entertainment & Pop Culture
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Watching Movies', 5, 22 )
      ,( 'Bingeing TV Shows', 5, 22 )
      ,( 'Anime', 5, 22 )
      ,( 'Podcasts', 5, 22 )
      ,( 'Concerts', 5, 22 )
      ,( 'Stand-Up Comedy', 5, 22 )
      ,( 'Trivia Nights', 5, 22 )
      ,( 'Book Clubs', 5, 22 )

-- Hobbies & Activities -> Culinary & Foodie
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Cooking', 5, 23 )
      ,( 'Baking', 5, 23 )
      ,( 'Trying New Restaurants', 5, 23 )
      ,( 'Wine Tasting', 5, 23 )
      ,( 'Coffee Shop Hopping', 5, 23 )
      ,( 'Home Brewing', 5, 23 )
      ,( 'Hosting Dinner Parties', 5, 23 )

-- Hobbies & Activities -> Social & Community
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Volunteering', 5, 24 )
      ,( 'Attending Meetups', 5, 24 )
      ,( 'Religious Activities', 5, 24 )
      ,( 'Club Memberships', 5, 24 )
      ,( 'Networking Events', 5, 24 )
      ,( 'Public Speaking', 5, 24 )
      ,( 'Political Activism', 5, 24 )

-- Hobbies & Activities -> Learning & Personal Growth
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Reading', 5, 25 )
      ,( 'Studying Languages', 5, 25 )
      ,( 'Learning Instruments', 5, 25 )
      ,( 'Online Courses', 5, 25 )
      ,( 'Self-Improvement', 5, 25 )
      ,( 'Career Development', 5, 25 )
      
-- Relationship Goals -> Intentions
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Just Browsing', 6, 26 )
      ,( 'Open to Chatting', 6, 26 )
      ,( 'Looking for New Friends', 6, 26 )
      ,( 'Casual Dating', 6, 26 )
      ,( 'Short-Term Relationship', 6, 26 )
      ,( 'Long-Term Relationship', 6, 26 )
      ,( 'Life Partner / Marriage', 6, 26 )
      ,( 'Open to Anything', 6, 26 )
      ,( 'Still Figuring It Out', 6, 26 )

-- Relationship Goals -> Relationship Style
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Monogamous', 6, 27 )
      ,( 'Open Relationship', 6, 27 )
      ,( 'Polyamorous', 6, 27 )
      ,( 'Ethical Non-Monogamy', 6, 27 )
      ,( 'Prefer to Discuss Later', 6, 27 )

-- Relationship Goals -> Family Plans
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Want Kids Someday', 6, 28 )
      ,( 'Have Kids', 6, 28 )
      ,( 'Don’t Want Kids', 6, 28 )
      ,( 'Not Sure About Kids', 6, 28 )
      ,( 'Open to Partner’s Preference', 6, 28 )

-- Relationship Goals -> Living Preferences
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Prefer Living Separately', 6, 29 )
      ,( 'Open to Moving In', 6, 29 )
      ,( 'Want to Build a Home Together', 6, 29 )
      ,( 'Travel Often Together', 6, 29 )
      ,( 'Maintain Personal Space', 6, 29 )

-- Relationship Goals -> Romantic Values
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Traditional', 6, 30 )
      ,( 'Modern', 6, 30 )
      ,( 'Affectionate', 6, 30 )
      ,( 'Reserved', 6, 30 )
      ,( 'Emotionally Open', 6, 30 )
      ,( 'Slow & Steady', 6, 30 )
      ,( 'Love at First Sight Believer', 6, 30 )

-- Personality Types -> Social Style
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Introvert', 7, 31 )
      ,( 'Extrovert', 7, 31 )
      ,( 'Ambivert', 7, 31 )
      ,( 'Shy Until Comfortable', 7, 31 )
      ,( 'Social Butterfly', 7, 31 )
      ,( 'Prefer One-on-One', 7, 31 )
      ,( 'Love Group Settings', 7, 31 )

-- Personality Types -> Emotional Style
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Empathic', 7, 32 )
      ,( 'Emotionally Reserved', 7, 32 )
      ,( 'Deep Thinker', 7, 32 )
      ,( 'Feels Deeply', 7, 32 )
      ,( 'Goofy / Lighthearted', 7, 32 )
      ,( 'Serious and Focused', 7, 32 )
      ,( 'Optimist', 7, 32 )
      ,( 'Realist', 7, 32 )
      ,( 'Idealist', 7, 32 )

-- Personality Types -> Decision-Making Style
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Spontaneous', 7, 33 )
      ,( 'Planner / Structured', 7, 33 )
      ,( 'Go With the Flow', 7, 33 )
      ,( 'Analytical', 7, 33 )
      ,( 'Impulsive (in a fun way)', 7, 33 )

-- Personality Types -> Energy & Vibe
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Calm and Chill', 7, 34 )
      ,( 'High-Energy', 7, 34 )
      ,( 'Creative Thinker', 7, 34 )
      ,( 'Problem Solver', 7, 34 )
      ,( 'Nurturing', 7, 34 )
      ,( 'Competitive', 7, 34 )
      ,( 'Adventurous', 7, 34 )
      ,( 'Grounded', 7, 34 )

-- Political Beliefs -> General Alignment
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Apolitical', 8, 35 )
      ,( 'Prefer Not to Say', 8, 35 )
      ,( 'Liberal / Left-Leaning', 8, 35 )
      ,( 'Conservative / Right-Leaning', 8, 35 )
      ,( 'Centrist / Moderate', 8, 35 )
      ,( 'Libertarian', 8, 35 )
      ,( 'Progressive', 8, 35 )
      ,( 'Green / Environmentalist', 8, 35 )
      ,( 'Socialist', 8, 35 )
      ,( 'Independent', 8, 35 )

-- Political Beliefs -> Engagement Level
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Actively Political', 8, 36 )
      ,( 'Occasionally Involved', 8, 36 )
      ,( 'Vote Only', 8, 36 )
      ,( 'Prefer to Avoid Politics', 8, 36 )

-- Political Beliefs -> Key Preferences
INSERT INTO TInterests ( strInterest, intInterestCategoryID, intInterestSubCategoryID )
VALUES ( 'Open to Political Differences', 8, 37 )
      ,( 'Prefer Similar Views', 8, 37 )
      ,( 'Willing to Discuss Respectfully', 8, 37 )
      ,( 'Not a Dealbreaker', 8, 37 )
      ,( 'Must Align Politically', 8, 37 )