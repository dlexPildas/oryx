using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using Printing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Printing.Services
{
    public class PieceHistoryService
    {
        private readonly IConfiguration Configuration;
        private readonly PiecesRecordRepository PiecesRecordRepository;
        private readonly PieceHistoryRepository PieceHistoryRepository;
        private readonly NonConformingPiecesRepository NonconformingPiecesRepository;
        private readonly NonConformitiesRepository NonConformitiesRepository;
        private readonly ImplantedPiecesRepository ImplantedPiecesRepository;
        private readonly RecordDeparturesRepository RecordDeparturesRepository;
        private readonly PieceLiberationRepository PieceLiberationRepository;

        public PieceHistoryService(IConfiguration configuration)
        {
            this.Configuration = configuration;
            PiecesRecordRepository = new PiecesRecordRepository(Configuration["OryxPath"] + "oryx.ini");
            PieceHistoryRepository = new PieceHistoryRepository(Configuration["OryxPath"] + "oryx.ini");
            NonconformingPiecesRepository = new NonConformingPiecesRepository(Configuration["OryxPath"] + "oryx.ini");
            NonConformitiesRepository = new NonConformitiesRepository(Configuration["OryxPath"] + "oryx.ini");
            ImplantedPiecesRepository = new ImplantedPiecesRepository(Configuration["OryxPath"] + "oryx.ini");
            RecordDeparturesRepository = new RecordDeparturesRepository(Configuration["OryxPath"] + "oryx.ini");
            PieceLiberationRepository = new PieceLiberationRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<PieceHistoryModel> GetPieceHistory(string of3peca)
        {
            OF3 of3 = await PiecesRecordRepository.Find(of3peca);
            OF1 of1 = await PieceHistoryRepository.FindFabricationOrder(of3peca);
            PieceData piecedata = await PieceHistoryRepository.FindPieceData(of3peca);
            List<OFA> ofaLst = await RecordDeparturesRepository.FindOfa(of3.Of3Ordem, of3.Of3Lote);

            PieceHistoryModel pieceHistoryModel = new PieceHistoryModel
            {
                OF3 = of3,
                Pr0Refer = piecedata.Pr0Refer,
                Pr0Desc = piecedata.Pr0Desc,
                Cr1Nome = (!string.IsNullOrEmpty(piecedata.Pr2Cor)
                ? piecedata.Cr1Nome
                : "Combinação/Jackard"),
                OfaLst = ofaLst
            };

            var ofh = await NonconformingPiecesRepository.Find(of3peca);
            if (ofh != null)
            {
                pieceHistoryModel.OfhDoc = ofh.Ofhdoc;
                pieceHistoryModel.OfgNome = await NonConformitiesRepository.FindOfgNome(ofh.Ofhmotivo);
            }

            if (string.IsNullOrEmpty(of3.Of3Ordem))
            {
                pieceHistoryModel.Of7Doc = await ImplantedPiecesRepository.FindOf7Doc(of3peca);
            }
            else
            {
                pieceHistoryModel.Of1Geracao = of1.Of1Geracao;
            }

            pieceHistoryModel.ShiftReportsLst = await PieceHistoryRepository.FindShiftReports(of3peca);
            pieceHistoryModel.Devolutions = await PieceHistoryRepository.FindDevolution(of3peca);
            pieceHistoryModel.GuideLst = await PieceHistoryRepository.FindGuide(of3peca);
            pieceHistoryModel.Of0Lst = await PieceLiberationRepository.FindLiberation(of3peca);
            pieceHistoryModel.LastExpeditionModel = await PieceHistoryRepository.FindLastExpedition(of3peca);
            
            return pieceHistoryModel;
        }
    }
}
