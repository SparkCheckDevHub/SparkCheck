
using SparkCheck.Data;
using SparkCheck.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace SparkCheck.Services
{
    public class MatchService
    {
        private readonly AppDbContext _context;
        private readonly ValidationService _validation;
        public MatchService(AppDbContext context, ValidationService validation)
        {
            _context = context;
            _validation = validation;
        }
        // ===================================================================================
        // Get latest active match, or null if none
        // ===================================================================================
        public async Task<TMatches?> GetLatestActiveMatchAsync(int? intUserID)
        {
            if (intUserID == null)
            {
                return null;
            }
            var query = (from match in _context.TMatches
                         join match_request in _context.TMatchRequests
                         on match.intMatchRequestID equals match_request.intMatchRequestID
                         where (match_request.intFirstUserID == intUserID || match_request.intSecondUserID == intUserID)
                             && match.dtmMatchEnded == null
                         orderby match.dtmMatchStarted descending
                         select match);
            return await query.FirstOrDefaultAsync();
        }
        // ===================================================================================
        // Get other user from match
        // ===================================================================================
        public async Task<TUsers?> GetOtherUserFromMatchAsync(int intUserID)
        {
            // Get the latest active match for the user
            var match = await GetLatestActiveMatchAsync(intUserID);
            if (match == null)
            {
                return null;
            }

            // Get the match request associated with the match
            var query = (from match_request in _context.TMatchRequests
                         where match_request.intMatchRequestID == match.intMatchRequestID
                         select match_request);
            var matchRequest = await query.FirstOrDefaultAsync();
            if (matchRequest == null)
            {
                return null;
            }

            // Is the first user our user? Return the other.
            if (matchRequest.intFirstUserID == intUserID)
            {
                return await _context.TUsers.FindAsync(matchRequest.intSecondUserID);
            }
            // First user must be the other user.
            else
            {
                return await _context.TUsers.FindAsync(matchRequest.intFirstUserID);
            }
        }
    }
}