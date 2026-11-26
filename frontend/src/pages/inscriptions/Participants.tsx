import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { inscriptionsApi } from "../../api/inscriptions";
import { groupsApi } from "../../api/groups";
import { extractErrorMessage } from "../../api/client";
import {
  Alert,
  Button,
  Card,
  EmptyState,
  Input,
  PageHeader,
  PageLoader,
  Select,
} from "../../components/ui";
import type { ParticipantDto, GroupDto, InscriptionDto } from "../../types";

const DOCUMENT_TYPES = ["CC", "TI", "CE", "PA"];

export default function Participants() {
  const [searchParams] = useSearchParams();
  const preselectedInscriptionId = searchParams.get("inscriptionId");

  const [myGroups, setMyGroups] = useState<GroupDto[]>([]);
  const [myInscriptions, setMyInscriptions] = useState<InscriptionDto[]>([]);
  const [selectedInscriptionId, setSelectedInscriptionId] = useState<number | null>(
    preselectedInscriptionId ? Number(preselectedInscriptionId) : null
  );
  const [participants, setParticipants] = useState<ParticipantDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  /* Cargar inscripciones del grupo */
  useEffect(() => {
    const init = async () => {
      setLoading(true);
      setError("");
      try {
        const [insc, grps] = await Promise.all([
          inscriptionsApi.getMyInscriptions(),
          groupsApi.getMyGroups(),
        ]);
        setMyInscriptions(insc);
        setMyGroups(grps);
      } catch (err) {
        setError(extractErrorMessage(err));
      } finally {
        setLoading(false);
      }
    };
    init();
  }, []);

  /* Cargar participantes cuando se selecciona inscripción */
  useEffect(() => {
    if (selectedInscriptionId === null) return;
    const fetchParticipants = async () => {
      setLoading(true);
      setError("");
      try {
        const data = await inscriptionsApi.getParticipants(selectedInscriptionId);
        setParticipants(data.length > 0 ? data : [emptyParticipant()]);
      } catch (err) {
        setError(extractErrorMessage(err));
        setParticipants([emptyParticipant()]);
      } finally {
        setLoading(false);
      }
    };
    fetchParticipants();
  }, [selectedInscriptionId]);

  const emptyParticipant = (): Partial<ParticipantDto> => ({
    firstName: "",
    lastName: "",
    documentNumber: "",
    birthDate: "",
  });

  const addRow = () =>
    setParticipants((prev) => [...prev, emptyParticipant() as ParticipantDto]);

  const removeRow = (idx: number) =>
    setParticipants((prev) => prev.filter((_, i) => i !== idx));

  const setField = (
    idx: number,
    field: keyof ParticipantDto,
    value: string
  ) => {
    setParticipants((prev) =>
      prev.map((p, i) => (i === idx ? { ...p, [field]: value } : p))
    );
  };

  const handleSave = async () => {
    if (selectedInscriptionId === null) return;
    // Validar que todos los campos obligatorios estén llenos
    for (const p of participants) {
      if (!p.firstName?.trim() || !p.lastName?.trim()) {
        setError("Todos los participantes deben tener nombre y apellido.");
        return;
      }
    }
    setSaving(true);
    setError("");
    setSuccess("");
    try {
      await inscriptionsApi.updateParticipants(selectedInscriptionId, {
        participants,
      });
      setSuccess("¡Participantes guardados correctamente!");
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <PageLoader />;

  const activeInscriptions = myInscriptions.filter(
    (i) => i.status === "Approved" || i.status === "Pending"
  );

  return (
    <div className="container page">
      <PageHeader
        title="Participantes"
        subtitle="Registra y actualiza los integrantes de tu grupo para cada inscripción"
      />

      {error && <Alert tone="error">{error}</Alert>}
      {success && <Alert tone="success">{success}</Alert>}

      {/* Selector de inscripción */}
      <Card>
        <Select
          label="Selecciona una inscripción"
          value={selectedInscriptionId ?? ""}
          onChange={(e) =>
            setSelectedInscriptionId(e.target.value ? Number(e.target.value) : null)
          }
        >
          <option value="">— Elige una inscripción —</option>
          {activeInscriptions.map((i) => (
            <option key={i.id} value={i.id}>
              {i.eventTitle ?? i.eventName ?? `Evento #${i.eventId}`} —{" "}
              {i.groupName ?? `Grupo #${i.groupId}`} ({i.status})
            </option>
          ))}
        </Select>
      </Card>

      {selectedInscriptionId === null && (
        <EmptyState
          title="Selecciona una inscripción"
          message="Elige una inscripción activa para gestionar sus participantes."
        />
      )}

      {selectedInscriptionId !== null && (
        <Card>
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              marginBottom: "1rem",
            }}
          >
            <h3>Participantes ({participants.length})</h3>
            <Button variant="secondary" onClick={addRow}>
              + Agregar participante
            </Button>
          </div>

          {participants.length === 0 ? (
            <EmptyState title="Sin participantes" message="Agrega al menos un participante." />
          ) : (
            <div className="table-responsive">
              <table className="table">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Nombres *</th>
                    <th>Apellidos *</th>
                    <th>Tipo doc.</th>
                    <th>N° Documento</th>
                    <th>Fecha de nacimiento</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  {participants.map((p, idx) => (
                    <tr key={idx}>
                      <td>{idx + 1}</td>
                      <td>
                        <input
                          className="input input--sm"
                          value={p.firstName ?? ""}
                          onChange={(e) => setField(idx, "firstName", e.target.value)}
                          placeholder="Nombres"
                          required
                        />
                      </td>
                      <td>
                        <input
                          className="input input--sm"
                          value={p.lastName ?? ""}
                          onChange={(e) => setField(idx, "lastName", e.target.value)}
                          placeholder="Apellidos"
                          required
                        />
                      </td>
                      <td>
                        <select
                          className="select select--sm"
                          value={(p as any).documentType ?? "CC"}
                          onChange={(e) =>
                            setField(idx, "documentType" as keyof ParticipantDto, e.target.value)
                          }
                        >
                          {DOCUMENT_TYPES.map((dt) => (
                            <option key={dt} value={dt}>
                              {dt}
                            </option>
                          ))}
                        </select>
                      </td>
                      <td>
                        <input
                          className="input input--sm"
                          value={p.documentNumber ?? ""}
                          onChange={(e) =>
                            setField(idx, "documentNumber", e.target.value)
                          }
                          placeholder="Documento"
                        />
                      </td>
                      <td>
                        <input
                          className="input input--sm"
                          type="date"
                          value={p.birthDate ? p.birthDate.substring(0, 10) : ""}
                          onChange={(e) => setField(idx, "birthDate", e.target.value)}
                        />
                      </td>
                      <td>
                        <Button
                          variant="ghost"
                          onClick={() => removeRow(idx)}
                          disabled={participants.length <= 1}
                        >
                          ✕
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          <div className="form-actions" style={{ marginTop: "1.5rem" }}>
            <Button loading={saving} onClick={handleSave}>
              💾 Guardar participantes
            </Button>
          </div>
        </Card>
      )}
    </div>
  );
}
