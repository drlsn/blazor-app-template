using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using Corelibs.MongoDB;
using Mediator;
using MongoDB.Driver;
using MyApp.Entities.Plans;
using MyApp.Entities.Users;
using MyApp.UseCases.Plans;

namespace MyApp.Infrastructure.UseCases.Queries
{
    public class GetOwnPlansQueryHandler : IQueryHandler<GetOwnPlansQuery, Result<GetOwnPlansQueryResponse>>
    {
        private readonly IClientSessionHandle _session;
        private readonly MongoConnection _mongoConnection;
        private readonly IAccessorAsync<CurrentUser> _currentUserAccessor;

        public GetOwnPlansQueryHandler(
            IClientSessionHandle session,
            MongoConnection mongoConnection,
            IAccessorAsync<CurrentUser> currentUserAccessor)
        {
            _session = session;
            _mongoConnection = mongoConnection;
            _currentUserAccessor = currentUserAccessor;
        }

        public async ValueTask<Result<GetOwnPlansQueryResponse>> Handle(GetOwnPlansQuery query, CancellationToken cancellationToken)
        {
            var result = Result<GetOwnPlansQueryResponse>.Success();

            var currentUser = await _currentUserAccessor.Get();
            if (currentUser is null)
                return result.Fail();

            var collection = _mongoConnection.Database.GetCollection<Plan>("plans");

            var userId = new UserId(currentUser.Id);
            var filter = Builders<Plan>.Filter.Eq(x => x.UserId, userId);

            var projection = Builders<Plan>.Projection.Include(x => x.Name);

            var doc = collection.Find(filter).Project(projection).FirstOrDefault();

            throw new NotImplementedException();
        }
    }
}
